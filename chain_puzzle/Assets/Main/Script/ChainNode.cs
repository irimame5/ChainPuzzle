using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChainNode : MonoBehaviour {

    [SerializeField]
    GameObject chain;
    [SerializeField]
    ChainNode[] connectedNodes;

	void Start () {
        
    }

    private void OnDrawGizmos()
    {
        const float connectShirtLength = 0.2f;
        foreach(var node in connectedNodes)
        {
            var direction = node.transform.position - transform.position;
            var orthogonalVector = Vector3.Cross(direction,transform.forward);
            orthogonalVector.Normalize();
            orthogonalVector *= connectShirtLength;
            DrawAllow(transform.position + orthogonalVector, direction);
        }
    }

    void DrawAllow(Vector3 from,Vector3 direction)
    {
        const float AllowTopLength= 0.3f;
        const float AllowRadius = 20f;
        Gizmos.color = Color.black;

        var toPosition = from + direction;

        var tmp = Quaternion.Euler(0, 0, AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        tmp = Quaternion.Euler(0, 0, -AllowRadius) * -direction;
        tmp = tmp.normalized * AllowTopLength;
        Gizmos.DrawRay(toPosition, tmp);

        Gizmos.DrawRay(from, direction);
    }

    void Update () {
		
	}


    public IEnumerator Connect(ChainNode conectNode,GameObject chainObject)
    {
        float connectingTime = TestSceneManager.Instance.GameParameter.ChainConnectTime;
        float timer = 0f;

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
    }

    public void PointerDown()
    {
        if (TestSceneManager.Instance.ChainNodes.Count == 0)
        {
            TestSceneManager.Instance.AddChainNode(this);
        }
        else
        {
            var previousChainNode = TestSceneManager.Instance.ChainNodes.Last().ChainNode;
            if (previousChainNode == this)
            {
                print("前のノードと同じノードが選択されました");
                return;
            }

            var chainObject = Instantiate(chain, transform);
            StartCoroutine(previousChainNode.Connect(this, chainObject));
            TestSceneManager.Instance.AddChainNode(this, chainObject);
        }
    }
}
