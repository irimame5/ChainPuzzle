using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSceneManager : MonoSingleton<TestSceneManager> {

    [SerializeField]
    List<ConnectObject> connectObjects = new List<ConnectObject>();
    public List<ConnectObject> ConnectObjects
    {
        get { return connectObjects; }
    }

    [SerializeField]
    GameParameter gameParameter;
    [SerializeField]
    GameObject lastChainNodeImagePrefab;
    [SerializeField]
    ChainEdge[] chainEdges;
    [SerializeField]
    GameObject gameClearUI;

    GameObject lastChainNodeImage;
    public GameParameter GameParameter
    {
        get { return gameParameter; }
    }

	void Start () {
		
	}

    public bool SerchConnectedEdge(ChainEdge chainEdge)
    {
        foreach(var connectedObject in connectObjects)
        {
            if (connectedObject == chainEdge)
            {
                return true;
            }
        }
        return false;
    }

    public void AddChainNode(ChainNode chainNode,ChainEdge chainEdge=null)
    {
        const float ChainNodeImageZ = -0.6f;//モデルに隠れないための調整分;

        var pos = chainNode.transform.position;
        pos.z = ChainNodeImageZ;
        if (connectObjects.Count == 0)
        {
            lastChainNodeImage = Instantiate(lastChainNodeImagePrefab,transform);
            lastChainNodeImage.transform.position = pos;
            connectObjects.Add(chainNode);
            return;
        }
        Debug.Assert(chainEdge != null);
        connectObjects.Add(chainEdge);
        lastChainNodeImage.transform.position = pos;
        connectObjects.Add(chainNode);
    }

    public void RemoveChainNode()
    {
        if(connectObjects.Count == 0)
        {
            return;
        }
        Debug.Assert(typeof(ChainNode) == connectObjects.Last().GetType());
        connectObjects.RemoveAt(connectObjects.Count-1);
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
        lastChainNodeImage.transform.position = connectObjects.Last().transform.position;
    }

    public bool CheckAllEdgePass()
    {
        foreach(var edge in chainEdges)
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

    public void GameClear()
    {
        print("GameClear");
        gameClearUI.SetActive(true);
    }

    [ContextMenu("SerchAllEdge")]
    void SerchAllEdge()
    {
        chainEdges = FindObjectsOfType<ChainEdge>();
    }
}
