﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Flags]
public enum ChainNodeAttribute
{
    Attack = 1 << 0,
    Magic = 1 << 1,  
    Guard = 1 << 2,
    Recovery = 1 << 3,
}

public class ChainNode : ConnectObject
{

    [SerializeField]
    GameObject chain;
    [SerializeField]
    GameObject ringEffect;
    [SerializeField]
    ChainEdge[] connectedChainEdges;
    [SerializeField]
    AudioClip connectChainSound;
    [SerializeField]
    AudioClip chainExtendSound;
    [SerializeField,EnumFlags]
    ChainNodeAttribute nodeAttribute;
    [SerializeField]
    GameObject attackNodeAttributeEffect;

    public ChainNodeAttribute NodeAttribute
    {
        get { return nodeAttribute; }
    }

    void Start () {
        if (nodeAttribute == ChainNodeAttribute.Attack)
        {
            Instantiate(attackNodeAttributeEffect, transform);
        }
    }

    private void OnDrawGizmos()
    {
        const float connectShirtLength = 0.2f;
        foreach(var node in connectedChainEdges)
        {
            var direction = node.transform.position - transform.position;
            var orthogonalVector = Vector3.Cross(direction,transform.forward);
            orthogonalVector.Normalize();
            orthogonalVector *= connectShirtLength;
            Gizmos.color = Color.black;
            EditorExtension.DrawAllow(transform.position + orthogonalVector, direction);
        }
    }

    public void PointerDown()
    {
        if (SequanceManager.Instance.ConnectObjects.Count == 0)
        {
            SequanceManager.Instance.AddChainNode(this);
        }
        else
        {
            ChainNode lastChainNode = (ChainNode)SequanceManager.Instance.ConnectObjects.Last();
            if (lastChainNode == this)
            {
                print("前のノードと同じノードが選択されました");
                return;
            }
            var connectEdge = lastChainNode.SearchConnectEdge(this);
            if (connectEdge==null)
            {
                print("前のノードと接続されていないノードが選択されました");
                return;
            }
            bool b = SequanceManager.Instance.SerchConnectedEdge(connectEdge);
            if (b)
            {
                print("既に接続されているエッジを通ります");
                return;
            }
            StartCoroutine(lastChainNode.Connect(this, connectEdge));
            SequanceManager.Instance.AddChainNode(this, connectEdge);
        }
    }

    /// <summary>
    /// 接続してるエッジの先に引数のノードがあるか調べる
    /// </summary>
    /// <param name="connectNode">接続されているか調べるノード</param>
    /// <returns>接続されていればその接続エッジ，なければnull</returns>
    public ChainEdge SearchConnectEdge(ChainNode connectNode)
    {
        foreach(var connectEdge in connectedChainEdges)
        {
            var connectingNode = connectEdge.GetConnectedOtherNode(connectNode);
            if (connectingNode!=null)
            {
                return connectEdge;
            }
        }
        return null;
    }

    void Update () {
		
	}

    /// <summary>
    /// 呼び出し側のノードから引数へ渡したノードへ接続する
    /// </summary>
    /// <param name="conectNode">接続先のノード</param>
    /// <param name="chainEdge">この子階層に生成する</param>
    /// <returns></returns>
    public IEnumerator Connect(ChainNode conectNode,ChainEdge chainEdge)
    {
        float connectingTime = MainGameSceneManager.Instance.GameParameter.ChainConnectTime;
        float timer = 0f;
        var chainObject = Instantiate(chain, chainEdge.transform);

        chainObject.transform.position = transform.position;
        var chainMaterial = chainObject.GetComponent<MeshRenderer>().material;
        float distance = Vector2.Distance(transform.position, conectNode.transform.position);
        chainObject.transform.up = (transform.position - conectNode.transform.position).normalized;

        SoundManager.Instance.PlaySE(chainExtendSound);
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            float rate = timer / connectingTime;
            float distanceRate = timer / connectingTime * distance;
            if (1 <= rate) { break; }
            var chainDelta = -chainObject.transform.up * distance * rate;
            chainObject.transform.position = transform.position + chainDelta;
            chainMaterial.SetFloat("_Extend", distanceRate);
        }
        chainObject.transform.position = transform.position + (-chainObject.transform.up * distance);
        chainMaterial.SetFloat("_Extend", distance);

        //ここで接続完了
        SoundManager.Instance.PlaySE(connectChainSound);
        CameraEffects.Instance.Shake();
        Instantiate(ringEffect, conectNode.transform.position, ringEffect.transform.rotation);

        bool b = SequanceManager.Instance.CheckAllEdgePass();
        if (b) { SequanceManager.Instance.CahinAllConect(); }
    }
}

