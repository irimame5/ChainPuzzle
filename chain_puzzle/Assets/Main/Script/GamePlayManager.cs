using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全シーン共通の処理,DontDestroy
/// 名前をGameManagerにするとUnityさんとかぶってギアのアイコンになるのでやめた
/// </summary>
public class GamePlayManager : MonoSingleton<GamePlayManager>
{

	protected override void SubAwake ()
	{
        DontDestroyOnLoad(gameObject);
    }

	void Update ()
	{
		
	}
}
