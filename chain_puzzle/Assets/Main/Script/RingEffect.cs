using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RingEffect : MonoBehaviour
{
    [SerializeField]
    float expandScale;
    [SerializeField]
    float expandTime;

    SpriteRenderer spriteRenderer;
    Vector3 firstScale;
	void Start ()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
        firstScale = transform.lossyScale;
        Expand();
	}

    public void Expand()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(
            transform.DOScale(expandScale * Vector3.one, expandTime)
        );
        sequence.Join(spriteRenderer.DOFade(0, expandTime));

        sequence.OnComplete(() => { Destroy(gameObject); });
    }
}
