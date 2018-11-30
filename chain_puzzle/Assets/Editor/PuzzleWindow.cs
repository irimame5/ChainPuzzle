using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class PuzzleWindow : EditorWindow
{
    const float ButtonSpace= 1;
    const float NormalButtonSize = 25;
    const float BigButtonSize = 50;

    [InitializeOnLoadMethod]
    static void Initialize()
    {
        //重複登録しないか不安
        EditorApplication.playModeStateChanged += OnChangedPlayMode;
    }

    static void OnChangedPlayMode(PlayModeStateChange  state)
    {
        var data = Resources.Load<EditorPlayData>("EditorPlayData");

        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorSceneManager.playModeStartScene = null;
            data.Reset();

            var list = EditorBuildSettings.scenes.ToList();
            list.Remove(list.Last());
            EditorBuildSettings.scenes = list.ToArray();
        }
    }

    [MenuItem("Custom/PuzzleWindow")]
    static void ShowWindow()
    {
        GetWindow<PuzzleWindow>();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Update", GUILayout.Height(NormalButtonSize)))
        {
            PuzzleUpdate();
        }
        GUILayout.Space(ButtonSpace);

        if (GUILayout.Button("ノードを作成する", GUILayout.Height(NormalButtonSize)))
        {
            CreateNode();
        }
        GUILayout.Space(ButtonSpace);

        if (GUILayout.Button("ノードを接続する", GUILayout.Height(NormalButtonSize)))
        {
            ConnectNode();
        }
        GUILayout.Space(ButtonSpace);

        if (GUILayout.Button("ノードの接続を解除する", GUILayout.Height(NormalButtonSize)))
        {
            DisConnectNode();
        }
        GUILayout.Space(ButtonSpace);

        if (GUILayout.Button("テストプレイ", GUILayout.Height(BigButtonSize)))
        {
            TestPlayStart();
        }

        //ShowAttribute();

        GUI.color = Color.red;
        if (GUILayout.Button("緊急脱出ボタン\n(注意:緊急時以外は押さないでください)", GUILayout.Height(BigButtonSize)))
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

    static void PuzzleUpdate()
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
            foreach (var item in a)
            {
                UnityEditor.EditorUtility.SetDirty(item);
            }
        }
        //接続されているオブジェクトでmissingやヌルの場合は破棄
        foreach (var a in connectedEdges)
        {
            a.ToList().RemoveAll(item => item == null);
            foreach (var item in a)
            {
                UnityEditor.EditorUtility.SetDirty(item);
            }
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
                    foreach (var a in item.ConnectedChainEdges)
                    {
                        UnityEditor.EditorUtility.SetDirty(a);
                    }
                }
                DestroyImmediate(edge.gameObject);
                continue;
            }

            ChainEdge b;
            b = edge.ConnectedChainNodes[0].SearchConnectEdge(edge.ConnectedChainNodes[1]);
            if (b==null) { edge.ConnectedChainNodes[0].ConnectedChainEdges.Add(edge); }
            b = edge.ConnectedChainNodes[1].SearchConnectEdge(edge.ConnectedChainNodes[0]);
            if (b==null) { edge.ConnectedChainNodes[1].ConnectedChainEdges.Add(edge); }
            foreach (var item in edge.ConnectedChainNodes[0].ConnectedChainEdges)
            {
                UnityEditor.EditorUtility.SetDirty(item);
            }
            foreach (var item in edge.ConnectedChainNodes[1].ConnectedChainEdges)
            {
                UnityEditor.EditorUtility.SetDirty(item);
            }
        }

        //接続しているエッジがなくなったらそのノードは破棄
        foreach (var node in nodes)
        {
            if (node.ConnectedChainEdges.Count == 0)
            {
                DestroyImmediate(node.gameObject);
            }
        }

        var sequanceManager = FindObjectOfType<SequanceManager>();
        if (sequanceManager == null) { Debug.LogError("SequanceManagerが見つかりません Update失敗"); return; }
        sequanceManager.SerchAllEdge();
        sequanceManager.SerchAllNode();

        EditorUtility.SetDirty(sequanceManager);
    }

    static void CreateNode()
    {
        var nodePrefab = Resources.Load<GameObject>("ChainNode");
        var node = Instantiate(nodePrefab);
        var chainParent = GameObject.Find("Chains");
        if (chainParent == null)
        {
            Debug.LogWarning("Chainsという名のオブジェクトがいないため，みなしごで作成");
            return;
        }
        node.transform.parent = chainParent.transform;
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

    static void TestPlayStart()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        if (!activeScene.name.Contains("Seq"))
        {
            Debug.LogError("Sequanceのシーン以外では呼び出せません");
            return;
        }

        var editorPlayData = Resources.Load<EditorPlayData>("EditorPlayData");
        editorPlayData.DebugLoadSequanceName = activeScene.name;
        EditorUtility.SetDirty(editorPlayData);

        var buildSettingsActiveScene = new EditorBuildSettingsScene(activeScene.path, true);
        var list = EditorBuildSettings.scenes.ToList();
        list.Add(buildSettingsActiveScene);
        EditorBuildSettings.scenes = list.ToArray();

        const string StartSceneName = "MainGame";
        //guidが入ってる
        var scenes = AssetDatabase.FindAssets("t:Scene");
        string scenePath="";
        foreach (var sceneGuid in scenes)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName == StartSceneName)
            {
                scenePath = path;
            }
        }
        if (scenePath=="") { Debug.LogError(StartSceneName+"が見つかりません");return; }
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        EditorSceneManager.playModeStartScene = sceneAsset;
        EditorApplication.isPlaying = true;
    }

    /// <summary>
    /// 動かないため封印
    /// </summary>
    //static void ShowAttribute()
    //{
    //    if (Selection.transforms.Length == 0) { return; }

    //    var nodes = Selection.transforms.Select(item => item.GetComponent<ChainNode>()).ToArray();
    //    if (!nodes.All(item => item != null)) { return; }
    //    if (nodes.Length!= Selection.transforms.Length) { return; }

    //    ChainNodeAttribute[] flags = new ChainNodeAttribute[nodes.Length];
    //    for (int i = 0; i < nodes.Length; i++)
    //    {
    //        flags[i] = nodes[i].NodeAttribute;
    //    }

    //    foreach (ChainNodeAttribute attribute in System.Enum.GetValues(typeof(ChainNodeAttribute)))
    //    {
    //        var chainNodeAttributeName = System.Enum.GetName(typeof(ChainNodeAttribute), attribute);
    //        bool[] attributeCheck = flags.Select(item => HasFlag(item, attribute)).ToArray();//HasFlagは拡張メソッド化

    //        bool b = attributeCheck.Distinct().Count() == 1;//重複がなければtrue

    //        if (b)
    //        {
    //            bool val = EditorGUILayout.ToggleLeft(chainNodeAttributeName, attributeCheck.First());
    //            foreach (var node in nodes)
    //            {
    //                if (val)
    //                {
    //                    node.NodeAttribute = node.NodeAttribute | attribute;
    //                }
    //                else
    //                {
    //                    node.NodeAttribute = node.NodeAttribute & ~attribute;
    //                }
    //            }
    //        }
    //        else
    //        {//wipここが呼ばれない
    //            GUILayout.Label(chainNodeAttributeName + ": 選択されているオブジェクトの値が異なっています");
    //        }
    //    }
    //}
}
