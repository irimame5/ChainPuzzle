using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PuzzleWindow : EditorWindow
{

    [MenuItem("Custom/PuzzleWindow")]
    static void ShowWindow()
    {
        GetWindow<PuzzleWindow>();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Update"))
        {
            UpdateButtonPress();
        }

        if (GUILayout.Button("ノードを作成する"))
        {
            var nodePrefab = Resources.Load<GameObject>("ChainNode");
            var node = Instantiate(nodePrefab);
        }

        if (GUILayout.Button("ノードを接続する"))
        {
            ConnectNode();
        }

        if (GUILayout.Button("ノードの接続を解除する"))
        {
            DisConnectNode();
        }

        GUI.color = Color.red;
        if (GUILayout.Button("緊急脱出ボタン\n(注意:緊急時以外は押さないでください)"))
        {
            if (Random.Range(0, 2) == 0)
            {
                Debug.LogError("脱出失敗");
            }else
            {
                EditorApplication.Exit(0);
            }
        }
    }

    static void UpdateButtonPress()//wip
    {
        var nodes = GameObject.FindObjectsOfType<ChainNode>().ToList();
        var edges = GameObject.FindObjectsOfType<ChainEdge>().ToList();
        var connectedNodes = nodes.Select(item => item.ConnectedChainEdges).ToList();
        var connectedEdges = edges.Select(item => item.ConnectedChainNodes).ToList();

        //接続されているオブジェクトでmissingやヌルの場合は破棄
        foreach(var a in connectedNodes)
        {
            if (a.Count==0 || a == null)
            {
                continue;
            }
            a.RemoveAll(item => item == null);
        }
        //接続されているオブジェクトでmissingやヌルの場合は破棄
        foreach (var a in connectedEdges)
        {
            a.ToList().RemoveAll(item => item == null);
        }
        
        //接続しているノードが2個ではなかったらそのエッジは破棄
        edges.RemoveAll(item => item.ConnectedChainNodes.Length != 2);
        foreach (var edge in edges)
        {
            if (edge.ConnectedChainNodes.Length != 2)
            {
                //破棄されるエッジの接続先の自分への参照を消す
                foreach(var item in edge.ConnectedChainNodes)
                {
                    item.ConnectedChainEdges.ToList().RemoveAll(a => a == item);
                }
                DestroyImmediate(edge.gameObject);
            }
        }

        //接続しているエッジがなくなったらそのノードは破棄
        //foreach (var node in nodes)
        //{
        //    if (node.ConnectedChainEdges.Count == 0)
        //    {
        //        DestroyImmediate(node.gameObject);
        //    }
        //}
    }

    static void ConnectNode()
    {
        if (Selection.transforms.Length != 2)
        {
            Debug.LogWarning("ノードが二つ選択されていません");
            return;
        }

        var nodes = Selection.transforms.Select(item => item.GetComponent<ChainNode>()).ToArray();
        nodes.ToList().RemoveAll(item => item == null);
        if (nodes.Length != 2)
        {
            Debug.LogWarning("Chain Node がアタッチされているオブジェクトではありません");
            return;
        }

        //既に接続されていた場合
        if (nodes[0].SearchConnectEdge(nodes[1]) != null)
        {
            Debug.LogWarning("既に接続されています");
            return;
        }

        var edgePrefab = Resources.Load<GameObject>("ChainEdge");

        var edgeParent = nodes[0].transform.parent;
        var edgePos = Vector3.Lerp(nodes[0].transform.position, nodes[1].transform.position, 0.5f);
        GameObject edge = Instantiate(edgePrefab);
        edge.transform.position = edgePos;
        if (edgeParent != null) { edge.transform.parent = edgeParent; }
        var edgeComponent = edge.AddComponent<ChainEdge>();
        edgeComponent.SetChainNode(nodes);
        nodes[0].ConnectedChainEdges.Add(edgeComponent);
        nodes[1].ConnectedChainEdges.Add(edgeComponent);
    }

    static void DisConnectNode()
    {
        if (Selection.transforms.Length != 2)
        {
            Debug.LogWarning("ノードが二つ選択されていません");
            return;
        }

        var nodes = Selection.transforms.Select(item => item.GetComponent<ChainNode>()).ToArray();
        nodes.ToList().RemoveAll(item => item == null);
        if (nodes.Length != 2)
        {
            Debug.LogWarning("Chain Node がアタッチされているオブジェクトではありません");
            return;
        }

        var connectEdege = nodes[0].SearchConnectEdge(nodes[1]);
        if (connectEdege == null)
        {
            Debug.LogWarning("接続されていません");
            return;
        }

        nodes[0].ConnectedChainEdges.Remove(connectEdege);
        nodes[1].ConnectedChainEdges.Remove(connectEdege);
        DestroyImmediate(connectEdege.gameObject);
    }
}
