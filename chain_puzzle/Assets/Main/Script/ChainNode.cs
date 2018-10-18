using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChainNode : ConnectObject
{

    [SerializeField]
    GameObject chain;
    [SerializeField]
    ChainEdge[] connectedChainEdges;

    void Start () {
        
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
            ExtendMethods.DrawAllow(transform.position + orthogonalVector, direction);
        }
    }

    public void PointerDown()
    {
        if (TestSceneManager.Instance.ConnectObjects.Count == 0)
        {
            TestSceneManager.Instance.AddChainNode(this);
        }
        else
        {
            ChainNode lastChainNode = (ChainNode)TestSceneManager.Instance.ConnectObjects.Last();
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
            bool b = TestSceneManager.Instance.SerchConnectedEdge(connectEdge);
            if (b)
            {
                print("既に接続されているエッジを通ります");
                return;
            }
            StartCoroutine(lastChainNode.Connect(this, connectEdge));
            FireCounter.Instance.RemoveFire();
            TestSceneManager.Instance.AddChainNode(this, connectEdge);
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
    /// <param name="chainObject"></param>
    /// <returns></returns>
    public IEnumerator Connect(ChainNode conectNode,ChainEdge chainEdge)
    {
        float connectingTime = TestSceneManager.Instance.GameParameter.ChainConnectTime;
        float timer = 0f;
        var chainObject = Instantiate(chain, chainEdge.transform);

        chainObject.transform.position = transform.position;
        var chainSpriteRenderer = chainObject.GetComponent<SpriteRenderer>();
        float distance = Vector2.Distance(transform.position, conectNode.transform.position);
        chainObject.transform.up = (conectNode.transform.position- transform.position).normalized;
         Vector2 size;
        size = chainSpriteRenderer.size;
        size.y = 0f;
        chainSpriteRenderer.size = size;

        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            float rate = timer / connectingTime;
            float distanceRate = timer / connectingTime * distance;
            if (1 <= rate) { break; }
            var chainTop = transform.position + chainObject.transform.up * distanceRate;
            var chainPosition = (chainTop - transform.position) / 2;
            chainObject.transform.position = transform.position + chainPosition;
            size = chainSpriteRenderer.size;
            size.y = rate * distance;
            chainSpriteRenderer.size = size;
        }

        bool b = TestSceneManager.Instance.CheckAllEdgePass();
        if (b) { TestSceneManager.Instance.GameClear(); }
    }
}
