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
    GameObject damageParticleEffect;
    [SerializeField]
    GameObject attackEffect;
    [SerializeField]
    int attackPower = 10;
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

    /// <summary>
    /// 引数があるとContextMenuから呼べないので別関数にしている
    /// </summary>
    [ContextMenu("AttackTest")]
    void AttackTest()
    {
        Attack();
    }

    public void Attack(System.Action onComplete = null)
    {
        const float AttackLag = 2;
        const float OnCompleteLag = 1;

        System.Action action = () =>
        {
            MainGameSceneManager.Instance.DamageToPlayer(attackPower);
            CameraEffects.Instance.Shake();
        };
        if (onComplete != null)
        {
            action += () => 
            {
                DOVirtual.DelayedCall(OnCompleteLag,()=> { onComplete.Invoke(); });
            };
        }
        DOVirtual.DelayedCall(AttackLag, ()=> { action.Invoke(); });
    }

	public void Damage(int value)
    {
        const float DamageLag = 0.3f;
        const float DamageTextLag = 0.2f;
        DOVirtual.DelayedCall(DamageLag, () => 
        {
            Instantiate(damageParticleEffect, transform.position, Quaternion.identity);
            System.Action action = () =>
            {
                Instantiate(damageTextEffect)
                .GetComponent<DamageTextEffect>()
                .Initialize(transform.position, value);
            };
            DOVirtual.DelayedCall(DamageTextLag, () => { action.Invoke(); });

            hp -= value;
            if (hp <= 0)
            {
                hp = 0;
                slider.value = 0;
                Dead();
            }
            slider.value = hp;
        });
    }

    void Dead()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentsInChildren<Canvas>().Select(x => x.enabled = false);
    }
}
