using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameSceneManager : MonoSingleton<MainGameSceneManager>
{
    [SerializeField]
    GameParameter gameParameter;
    public GameParameter GameParameter
    {
        get { return gameParameter; }
    }
    [SerializeField]
    GameObject gameClearUI;
    [SerializeField]
    public Enemy Enemy;
    [SerializeField]
    AudioClip mainBgm;
    [SerializeField]
    int playerHp;
    [SerializeField]
    Slider hpBar;
    [SerializeField]
    GameObject damageTextEffect;
    [SerializeField,SceneName]
    string[] sequances;

    int sequanceIndex;
	void Start () {
        SoundManager.Instance.PlayBgmSingle(mainBgm);
        hpBar.maxValue = playerHp;
        hpBar.value = playerHp;

        LoadSequance();
    }

    public void LoadSequance()
    {
        if (sequanceIndex != 0)
        {
            SceneManager.UnloadSceneAsync(sequances[sequanceIndex-1]);
        }
        if (sequanceIndex == sequances.Length)
        {
            Clear();
            return;
        }
        SceneManager.LoadScene(sequances[sequanceIndex], LoadSceneMode.Additive);
        sequanceIndex++;
    }

    public void DamageToPlayer(int value)
    {
        //Instantiate(damageTextEffect)
        //.GetComponent<DamageTextEffect>()
        //.Initialize(transform.position, value);

        playerHp -= value;
        if (playerHp <= 0)
        {
            playerHp = 0;
            hpBar.value = 0;
            Dead();
        }
        hpBar.value = playerHp;
    }

    void Clear()
    {
        gameClearUI.SetActive(true);
    }

    void Dead()
    {

    }
}
