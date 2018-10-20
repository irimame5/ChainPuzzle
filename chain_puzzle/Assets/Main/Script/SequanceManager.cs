﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SequanceManager : MonoSingleton<SequanceManager>
{
    [SerializeField, Disable]
    List<ConnectObject> connectObjects = new List<ConnectObject>();
    public List<ConnectObject> ConnectObjects
    {
        get { return connectObjects; }
    }
    [SerializeField]
    GameObject lastChainNodeImagePrefab;

    [SerializeField,Disable]
    ChainEdge[] chainEdges;
    GameObject lastChainNodeImage;

    void Start ()
	{
		
	}

#if UNITY_EDITOR
    /// <summary>
    /// シーン作成時に実行しておく
    /// </summary>
    [ContextMenu("SerchAllEdge")]
    void SerchAllEdge()
    {
        chainEdges = null;
        chainEdges = FindObjectsOfType<ChainEdge>();
    }
#endif

    public bool SerchConnectedEdge(ChainEdge chainEdge)
    {
        foreach (var connectedObject in connectObjects)
        {
            if (connectedObject == chainEdge)
            {
                return true;
            }
        }
        return false;
    }

    public void AddChainNode(ChainNode chainNode, ChainEdge chainEdge = null)
    {
        const float ChainNodeImageZ = -0.6f;//モデルに隠れないための調整分;

        var pos = chainNode.transform.position;
        pos.z = ChainNodeImageZ;
        if (connectObjects.Count == 0)
        {
            lastChainNodeImage = Instantiate(lastChainNodeImagePrefab, transform);
            lastChainNodeImage.transform.position = pos;
            connectObjects.Add(chainNode);
            return;
        }
        Debug.Assert(chainEdge != null);
        connectObjects.Add(chainEdge);
        lastChainNodeImage.transform.position = pos;
        connectObjects.Add(chainNode);
        FireCounter.Instance.RemoveFire();
    }

    public void RemoveChainNode()
    {
        const float ChainNodeImageZ = -0.6f;//モデルに隠れないための調整分;

        if (connectObjects.Count == 0)
        {
            return;
        }
        Debug.Assert(typeof(ChainNode) == connectObjects.Last().GetType());
        connectObjects.RemoveAt(connectObjects.Count - 1);
        if (connectObjects.Count == 0)
        {
            Destroy(lastChainNodeImage);
            lastChainNodeImage = null;
            return;
        }
        Debug.Assert(typeof(ChainEdge) == connectObjects.Last().GetType());
        connectObjects.Last().GetComponent<ChainEdge>().RemoveEdge();
        connectObjects.RemoveAt(connectObjects.Count - 1);
        Debug.Assert(typeof(ChainNode) == connectObjects.Last().GetType());
        var pos = connectObjects.Last().transform.position;
        pos.z = ChainNodeImageZ;
        lastChainNodeImage.transform.position = pos;
        FireCounter.Instance.AddFire();

    }

    public bool CheckAllEdgePass()
    {
        foreach (var edge in chainEdges)
        {
            if (!CheckEdgePass(edge))
            {
                return false;
            }
        }
        return true;
    }
    bool CheckEdgePass(ChainEdge edge)
    {
        foreach (var connectedObject in connectObjects)
        {
            if (edge == connectedObject)
            {
                return true;
            }
        }
        return false;
    }

    public void CahinAllConect()
    {
        var cahiNodeAttributes =
            connectObjects.Where(x => x is ChainNode)
            .Select(x => x.GetComponent<ChainNode>()
            .NodeAttribute).ToArray();

        int damageSum = 0;
        int magicSum = 0;
        int guardSum = 0;
        int recoverySum = 0;

        //num1にnum2が含まれているか
        Func<ChainNodeAttribute, ChainNodeAttribute, bool> attributeCompare
            = (ChainNodeAttribute num1, ChainNodeAttribute num2) =>
            {
                var result = (num1 & num2) == num2;
                return result;
            };

        damageSum = cahiNodeAttributes.Where(x => attributeCompare(x, ChainNodeAttribute.Attack)).Count();
        magicSum = cahiNodeAttributes.Where(x => attributeCompare(x, ChainNodeAttribute.Magic)).Count();
        guardSum = cahiNodeAttributes.Where(x => attributeCompare(x, ChainNodeAttribute.Guard)).Count();
        recoverySum = cahiNodeAttributes.Where(x => attributeCompare(x, ChainNodeAttribute.Recovery)).Count();

        int damage = (int)(damageSum * MainGameSceneManager.Instance.GameParameter.DamageAttributeRate);
        MainGameSceneManager.Instance.Enemy.Damage(damage);
        MainGameSceneManager.Instance.LoadSequance();
    }
}
