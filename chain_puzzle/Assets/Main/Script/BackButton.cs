using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnityEventだと直接static classにアクセスできないためかませてる
/// </summary>
public class BackButton : MonoBehaviour
{
    public void Back()
    {
        SequanceManager.Instance.RemoveChainNode();
    }
}
