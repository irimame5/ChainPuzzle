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
    AudioClip damageSound;
    [SerializeField]
    AudioClip deadSound;
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
    void Awake()
    {
        MainGameSceneManager.Instance.Enemy = this;
    }
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
        const float AttackLag = 0.8f;
        const float OnCompleteLag = 1;

        StartCoroutine(AttackAnimation());
        yield return new WaitForSeconds(AttackLag);
        MainGameSceneManager.Instance.DamageToPlayer(attackPower);
        CameraEffects.Instance.PlayerDamage();

        if (onComplete != null)
        {
            yield return new WaitForSeconds(OnCompleteLag);
            onComplete.Invoke();
        }
    }
    IEnumerator AttackAnimation(float attackTime = 1)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackTime);
        animator.SetTrigger("Stand");
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

        hp -= value;
        if (hp <= 0)
        {
            hp = 0;
            slider.value = 0;
            Dead();
            yield break;
        }

        StartCoroutine(DamageAnimation());
        yield return new WaitForSeconds(DamageLag);
        Instantiate(damageParticleEffect, transform.position, Quaternion.identity);
        SoundManager.Instance.PlaySE(damageSound);

        yield return new WaitForSeconds(DamageTextLag);
        Instantiate(damageTextEffect)
        .GetComponent<DamageTextEffect>()
        .Initialize(transform.position, value);

        slider.value = hp;
    }
    IEnumerator DamageAnimation(float damageTime = 1)
    {
        animator.SetTrigger("Damage");
        yield return new WaitForSeconds(damageTime);
        animator.SetTrigger("Stand");
    }

    void Dead()
    {
        animator.SetTrigger("Dead");
        SoundManager.Instance.PlaySE(deadSound);
    }
}
