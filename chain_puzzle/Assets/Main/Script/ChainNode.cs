using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainNode : MonoBehaviour {

    [SerializeField]
    GameObject chain;

	void Start () {
		
	}
	
	void Update () {
		
	}


    public IEnumerator Connect(ChainNode conectNode)
    {
        const float ConnectingTime = 1f;


        float timer = 0f;
        var chainObject = Instantiate(chain, transform);
        var spriteRenderer = chainObject.GetComponent<SpriteRenderer>();
        float distance = Vector2.Distance(transform.position, conectNode.transform.position);
        chainObject.transform.up = transform.position - conectNode.transform.position;
         Vector2 size;
        size = spriteRenderer.size;
        size.y = 0f;
        spriteRenderer.size = size;

        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            float rate = timer / ConnectingTime;
            if (1 <= rate) { break; }
            size = spriteRenderer.size;
            size.y = rate * distance;
            spriteRenderer.size = size;
        }
    }

    public void PointerDown()
    {
        if (TestSceneManager.Instance.ChainNodes.Count == 0)
        {
            TestSceneManager.Instance.ChainNodes.Add(this);
            return;
        }
        var previousChain = TestSceneManager.Instance.ChainNodes[TestSceneManager.Instance.ChainNodes.Count - 1];
        StartCoroutine(previousChain.Connect(this));
        TestSceneManager.Instance.ChainNodes.Add(this);
    }
}
