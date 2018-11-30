using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEdge : ConnectObject
{
    [Disable]
    public bool IsPassed;
    [Disable]
    public List<GameObject> ConnectEffects = new List<GameObject>();
    [Disable]
    public GameObject ChainModel;

    [SerializeField]
    ChainNode[] connectedChainNodes = new ChainNode[2];
    public ChainNode[] ConnectedChainNodes
    {
        get { return connectedChainNodes; }
    }

    public void SetChainNode(ChainNode[] edge)
    {
        Debug.Assert(edge.Length == 2);
        connectedChainNodes = edge;
    }

    void Start () {
        Debug.Assert(connectedChainNodes.Length == 2);
	}

    public ChainNode GetConnectedOtherNode(ChainNode chainNode)
    {
        Debug.Assert(connectedChainNodes.Length == 2);
        if (connectedChainNodes[0] != chainNode
            && connectedChainNodes[1] != chainNode)
        {
            return null;
        }
        if(connectedChainNodes[0] == chainNode)
        {
            return connectedChainNodes[1];
        }
        else
        {
            return connectedChainNodes[0];
        }
    }

    public void RemoveEdge()
    {
        Destroy(ChainModel);
        ChainModel = null;
    }

    private void OnDrawGizmos()
    {
        const float connectShirtLength = 0.2f;
        foreach (var node in ConnectedChainNodes)
        {
            if (node == null) { continue; }
            var direction = node.transform.position - transform.position;
            var orthogonalVector = Vector3.Cross(direction, transform.forward);
            orthogonalVector.Normalize();
            orthogonalVector *= connectShirtLength;
            Gizmos.color = Color.black;
            EditorExtension.DrawAllow(transform.position + orthogonalVector, direction);
        }
    }

    void Update () {
		
	}
}
