using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全シーン共通の処理,DontDestroy
/// 名前をGameManagerにするとUnityさんとかぶってギアのアイコンになるのでやめた
/// </summary>
public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    [SerializeField, Disable,Tooltip("解放されているステージの数")]
    int openedStageNum = 1;
    public int OpenedStageNum
    {
        get { return openedStageNum; }
    }
    [SerializeField, Disable,Tooltip("次回ロードするステージのナンバー,初期値は-1")]
    public int loadStageNum = -1;
    public int LoadStageNum
    {
        get { return loadStageNum; }
    }

    protected override void SubAwake ()
	{
        DontDestroyOnLoad(gameObject);
    }

    public void StageClear()
    {
        openedStageNum++;
    }

    public void ResetLoadStageNum()
    {
        loadStageNum = -1;
    }

	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
