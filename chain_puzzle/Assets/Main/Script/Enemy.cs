using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    int hp;
    public int Hp
    {
        get { return hp; }
    }

	void Start ()
	{
		
	}

	public void Damage(int num)
    {
        hp -= num;
        if (hp <= 0)
        {
            hp = 0;
            Dead();
        }
    }

    void Dead()
    {

    }
}
