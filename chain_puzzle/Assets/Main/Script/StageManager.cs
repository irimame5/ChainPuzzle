using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager> {

    [SerializeField]
    AudioClip mainBgm;
    void Start () {
        SoundManager.Instance.PlayBgmSingle(mainBgm);
    }

    void Update () {
		
	}
}
