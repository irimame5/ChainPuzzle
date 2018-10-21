using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    DamageTextEffect damageTextEffect;
    [SerializeField]
    GameObject attackEffect;
    [SerializeField]
    GameObject attackParticleEffect;
    [SerializeField]
    float attackTime = 0.85f;
    [SerializeField,Tooltip("攻撃した球をずらす量,カメラから見えなくなってしまうの")]
    float attackEffectShift=1f;
    [SerializeField,Tooltip("x,y平面上にランダムに車室位置をずらすための半径")]
    float randomShiftRadius;

    [SerializeField]
    int hp;
    public int Hp
    {
        get { return hp; }
    }
    public bool DeadFlag
    {
        get { return hp <= 0; }
    }
    Slider slider;
	void Start ()
	{
        slider = GetComponentInChildren<Slider>();
        slider.maxValue = hp;
        slider.value = hp;
	}

    [ContextMenu("Attack")]
    public void Attack()
    {
        //Debug.Break();
        var instAttackEffect = Instantiate(attackEffect, transform.position, attackEffect.transform.rotation);
        var cameraPos = Camera.main.transform.position;
        var cameraToEnemy = transform.position - cameraPos;
        cameraToEnemy.Normalize();
        Vector3 randomShiftVector
            = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),0f);//本当はNormalizeする
        var targetPos = cameraPos + cameraToEnemy * attackEffectShift + randomShiftVector * randomShiftRadius;
        var sequance = DOTween.Sequence();
        sequance.Append
        (
            instAttackEffect.transform.DOMove(targetPos, attackTime).SetEase(Ease.Linear)
         );
        sequance.OnComplete
        (
            () => 
            {
                Instantiate(attackParticleEffect, targetPos, attackParticleEffect.transform.rotation);
                Destroy(instAttackEffect);
            }
        );

    }

	public void Damage(int value)
    {
        Instantiate(damageTextEffect)
        .GetComponent<DamageTextEffect>()
        .Initialize(transform.position,value);

        hp -= value;
        if (hp <= 0)
        {
            hp = 0;
            slider.value = 0;
            Dead();
        }
        slider.value = hp;
    }

    void Dead()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentsInChildren<Canvas>().Select(x => x.enabled = false);
    }
}
