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
    public Enemy enemy;
    public Enemy Enemy
    {
        get { return enemy; }
    }
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

    /// <summary>
    /// ロード中のシーケンスがなければ-1
    /// </summary>
    int sequanceIndex = -1;
	void Start () {
        SoundManager.Instance.PlayBgmSingle(mainBgm);
        hpBar.maxValue = playerHp;
        hpBar.value = playerHp;

        LoadRandomSequance();
    }

    public void LoadNextSequance()
    {
        if (sequanceIndex == -1)
        {
            sequanceIndex = 0;
        }else
        {
            SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);
            sequanceIndex++;
        }
        if (sequanceIndex == sequances.Length)
        {
            Clear();
            return;
        }
        SceneManager.LoadScene(sequances[sequanceIndex], LoadSceneMode.Additive);
    }
    public void LoadRandomSequance()
    {
        if (sequanceIndex == -1)
        {
            sequanceIndex = UnityEngine.Random.Range(0, sequances.Length);
        }
        else
        {
            SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);
            sequanceIndex = UnityEngine.Random.Range(0,sequances.Length);
        }
        SceneManager.LoadScene(sequances[sequanceIndex], LoadSceneMode.Additive);
    }
    public void UnLoadSequance()
    {
        if (sequanceIndex == -1)
        {
            Debug.LogWarning("LoadされていないのにSequanceのunLoadが呼び出されました");
            return;
        }
        SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);
        sequanceIndex = -1;
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

    public void EndSequance()
    {
        if (ClearCheck())
        {
            Clear();
            UnLoadSequance();
            return;
        }
        LoadRandomSequance();
    }

    bool ClearCheck()
    {
        if (enemy.DeadFlag)
        {
            return true;
        }else
        {
            return false;
        }
    }

    void Clear()
    {
        gameClearUI.SetActive(true);
    }

    void Dead()
    {

    }
}
