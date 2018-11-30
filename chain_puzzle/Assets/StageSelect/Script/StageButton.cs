using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour {

    [SerializeField,Disable,Tooltip("このボタンを押すとこのステージを読み込む")]
    public int LoadStageNum = -1;

	public void Pressed()
    {
        Debug.Assert(LoadStageNum != -1);
        GamePlayManager.Instance.loadStageNum = LoadStageNum;
    }
}
