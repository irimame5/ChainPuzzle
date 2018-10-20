using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    int hp;
    [SerializeField]
    DamageTextEffect damagetextEffect;
    public int Hp
    {
        get { return hp; }
    }

    Slider slider;
	void Start ()
	{
        slider = GetComponentInChildren<Slider>();
        slider.maxValue = hp;
        slider.value = hp;
	}

	public void Damage(int value)
    {
        Instantiate(damagetextEffect)
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

    }
}
