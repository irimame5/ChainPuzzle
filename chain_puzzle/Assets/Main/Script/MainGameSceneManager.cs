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
    [SerializeField]
    AudioClip damageSound;
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

    /// <summary>
    /// Sequanceの状況によっては押されたくないので，
    /// ここでbackしていいか調べながら呼び出す
    /// </summary>
    public void BackButtonPressed()
    {
        if (sequanceIndex == -1)//-1だとSequanceがないのでSequanceManagerを参照できない
        {
            return;
        }
        SequanceManager.Instance.RemoveChainNode();
    }

    //public void LoadNextSequance()
    //{
    //    if (sequanceIndex == -1)
    //    {
    //        sequanceIndex = 0;
    //    }else
    //    {
    //        SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);
    //        sequanceIndex++;
    //    }
    //    if (sequanceIndex == sequances.Length)
    //    {
    //        Clear();
    //        return;
    //    }
    //    SceneManager.LoadScene(sequances[sequanceIndex], LoadSceneMode.Additive);
    //}
    public void LoadRandomSequance()
    {
        if (sequanceIndex == -1)
        {
            sequanceIndex = UnityEngine.Random.Range(0, sequances.Length);
        }
        else
        {
            SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);//UnLoadSceneにしたいけど,Asyncを使えって言われた
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
        SceneManager.UnloadSceneAsync(sequances[sequanceIndex]);//UnLoadSceneにしたいけど,Asyncを使えって言われた
        sequanceIndex = -1;
    }

    public IEnumerator EndSequance()
    {
        const float AttackedLag = 0.8f;

        UnLoadSequance();

        if (ClearCheck())
        {
            Clear();
            yield break;
        }
        yield return StartCoroutine(enemy.Attack());
        yield return new WaitForSeconds(AttackedLag);
        LoadRandomSequance();
    }

    public IEnumerator DamageToEnemy(int damage)
    {
        yield return StartCoroutine(SequanceManager.Instance.NodeMaterialGlow());

        yield return StartCoroutine(enemy.Damage(damage));
        StartCoroutine(EndSequance());
    }

    public void DamageToPlayer(int value)
    {
        SoundManager.Instance.PlaySE(damageSound);

        playerHp -= value;
        if (playerHp <= 0)
        {
            playerHp = 0;
            hpBar.value = 0;
            Dead();
        }
        hpBar.value = playerHp;
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
