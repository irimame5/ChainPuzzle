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

    [SerializeField, Disable]
    ChainEdge[] chainEdges;
    [SerializeField, Disable]
    ChainNode[] chainNodes;

    GameObject lastChainNodeImage;

    void Start ()
	{
		
	}

#if UNITY_EDITOR
    /// <summary>
    /// シーン作成時に実行しておく
    /// 配列の長さをへっこうしてから実行しないと値が保存されないらしい
    /// </summary>
    [ContextMenu("SerchAllEdge")]
    public void SerchAllEdge()
    {
        chainEdges = null;
        chainEdges = FindObjectsOfType<ChainEdge>();
        foreach (var edge in chainEdges)
        {
            UnityEditor.EditorUtility.SetDirty(edge);
        }
    }
    [ContextMenu("SerchAllNode")]
    public void SerchAllNode()
    {
        chainNodes = null;
        chainNodes = FindObjectsOfType<ChainNode>();
        foreach(var node in chainNodes)
        {
            UnityEditor.EditorUtility.SetDirty(node);
        }
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

    /// <summary>
    /// このノードが行き止まりかどうか
    /// </summary>
    /// <param name="node">調べるノード</param>
    /// <returns>行き止まりならtrue</returns>
    public bool CheckDeadEndNode(ChainNode node)
    {
        foreach (var edge in node.ConnectedChainEdges)
        {
            if (!CheckEdgePass(edge))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// すべてのエッジを通っているか
    /// </summary>
    /// <returns>すべてのエッジを通っていればtrue</returns>
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

    public void ChainAllConect()
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
        StartCoroutine(MainGameSceneManager.Instance.DamageToEnemy(damage));
    }

    [ContextMenu("NodematerialGlowTest")]
    public void NodematerialGlowTest()
    {
        StartCoroutine(NodeMaterialGlow());
    }

    public IEnumerator NodeMaterialGlow()
    {
        const float MaxEmmisionTime = 1f;
        const float ChageClearTime = 0.4f;

        var chainRenderers = chainEdges.Select(chain => chain.GetComponentInChildren<MeshRenderer>()).Where(x => x != null);
        var renderers = chainRenderers
            .Concat(chainNodes
            .Select(node=>node.GetComponentInChildren<MeshRenderer>()))
            .ToList();

        //モデルたちを光らせていく
        float timer = 0;
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (MaxEmmisionTime <= timer) { break; }
            float rate = timer / MaxEmmisionTime;
            Color currentColor = new Color(rate, rate, rate);
            foreach (var meshRenderer in renderers)
            {
                meshRenderer.material.SetColor("_EmissionColor", currentColor);
            }
        }
        foreach (var meshRenderer in renderers)
        {
            meshRenderer.material.SetColor("_EmissionColor", Color.white);
        }

        Destroy(lastChainNodeImage);
        var connectEffects = chainEdges.SelectMany(x => x.ConnectEffects)
            .Select(x=>x.GetComponentInChildren<ParticleSystem>());//エフェクトを破棄
        foreach(var connectEffect in connectEffects)
        { connectEffect.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); }
        var attributeEffects = chainNodes.SelectMany(x => x.AttributeEffects)
            .Select(x => x.GetComponent<ParticleSystem>());//エフェクトを破棄
        foreach (var attributeEffect in attributeEffects)
        { attributeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }


        //モデルたちを透過させていく
        timer = 0;
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (ChageClearTime <= timer) { break; }
            float rate = timer / ChageClearTime;
            foreach (var meshRenderer in renderers)
            {
                Color materialColor = meshRenderer.material.GetColor("_Color");
                materialColor.a = 1 - rate;
                meshRenderer.material.SetColor("_Color", materialColor);
            }
        }
        foreach (var meshRenderer in renderers)
        {
            meshRenderer.material.SetColor("_Color", Color.clear);
        }
    }
}