using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectSceneManager : MonoBehaviour
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
