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

    Animator animator;
    Slider slider;
	void Start ()
	{
        animator = GetComponentInChildren<Animator>();
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
        StartCoroutine(Attack());
    }
    public IEnumerator Attack(System.Action onComplete = null)
    {
        const float AttackLag = 1;
        const float OnCompleteLag = 1;

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(AttackLag);
        MainGameSceneManager.Instance.DamageToPlayer(attackPower);
        CameraEffects.Instance.PlayerDamage();

        if (onComplete != null)
        {
            yield return new WaitForSeconds(OnCompleteLag);
            onComplete.Invoke();
        }
    }

    /// <summary>
    /// 引数があるとContextMenuから呼べないので別関数にしている
    /// </summary>
    [ContextMenu("DamageTest")]
    void DamageTest()
    {
        StartCoroutine(Damage(10));
    }
    public IEnumerator Damage(int value)
    {
        const float DamageLag = 0.3f;
        const float DamageTextLag = 0.2f;

        animator.SetTrigger("Damage");
        yield return new WaitForSeconds(DamageLag);
        Instantiate(damageParticleEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(DamageTextLag);
        Instantiate(damageTextEffect)
        .GetComponent<DamageTextEffect>()
        .Initialize(transform.position, value);

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
