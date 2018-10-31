using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全シーン共通の処理,DontDestroy
/// 名前をGameManagerにするとUnityさんとかぶってギアのアイコンになるのでやめた
/// </summary>
public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    [SerializeField, Disable]
    int openedStageNum = 1;
    public int OpenedStageNum
    {
        get { return openedStageNum; }
    }

	protected override void SubAwake ()
	{
        DontDestroyOnLoad(gameObject);
    }

    public void StageClear()
    {
        openedStageNum++;
    }

	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
