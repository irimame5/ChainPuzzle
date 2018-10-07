using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSceneManager : MonoSingleton<TestSceneManager> {

    [SerializeField]
    List<ChainData> chainNodes = new List<ChainData>();
    public List<ChainData> ChainNodes
    {
        get { return chainNodes; }
    }

    [SerializeField]
    GameParameter gameParameter;
    public GameParameter GameParameter
    {
        get { return gameParameter; }
    }

	void Start () {
		
	}

    public void AddChainNode(ChainNode chainNode,GameObject chainObject=null)
    {
        if (chainNodes.Count == 0)
        {
            chainNodes.Add(new ChainData(chainNode));
            return;
        }
        Debug.Assert(chainNodes.Last().Chain == null);
        Debug.Assert(chainObject != null);
        chainNodes.Last().Chain = chainObject;
        chainNodes.Add(new ChainData(chainNode));
    }

    public void RemoveChainNode()
    {
        chainNodes.RemoveAt(chainNodes.Count-1);//最後の接続を外し
        Destroy(chainNodes.Last().Chain);//新たな最後のChainを破棄する
        chainNodes.Last().Chain = null;
    }
	
	void Update () {
		
	}
}
