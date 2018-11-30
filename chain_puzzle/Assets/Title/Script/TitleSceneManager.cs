using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoSingleton<TitleSceneManager>
{
    [SerializeField]
    AudioClip bgm;
	void Start ()
	{
        SoundManager.Instance.PlayBgmSingle(bgm);
	}

	void Update ()
	{
		
	}
}
