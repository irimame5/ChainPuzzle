using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChainData{
    public ChainNode ChainNode;
    public GameObject Chain;

    public ChainData(ChainNode chainNode)
    {
        ChainNode = chainNode;
        Chain = null;
    }
}
