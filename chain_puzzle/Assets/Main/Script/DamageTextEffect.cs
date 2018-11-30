using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField]
    Vector2 moveDistance = new Vector2(0, 5);
    [SerializeField]
    float moveTime = 2;
    [SerializeField]
    string preText="敵に";
    [SerializeField]
    string postText="Damage";


    public void Initialize(Vector3 position,int value)
    {
        transform.position = position;

        //Instantiateされ,Start前にすぐ呼ばれるのでここで GetComp
        GetComponent<TextMeshPro>().text = preText + value.ToString() + postText;
        transform.DOMove(position+(Vector3)moveDistance, moveTime)
            .SetEase(Ease.OutBack)
            .OnComplete(() => { Destroy(gameObject); });
    }
}
