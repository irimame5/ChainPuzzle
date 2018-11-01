using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectSceneManager : MonoSingleton<StageSelectSceneManager>
{
    [SerializeField]
    AudioClip bgm;
    [SerializeField]
    Transform stageButtons;
    [SerializeField]
    GameObject stageButtonPrefab;
    [SerializeField,Tooltip("表示するステージ数,解放されていないステージも含む")]
    int maxStage = 8;

	void Start ()
	{
        SoundManager.Instance.PlayBgmSingle(bgm);
        GamePlayManager.Instance.ResetLoadStageNum();
        for (int i = 0; i < maxStage; i++)
        {
            var button = Instantiate(stageButtonPrefab, stageButtons);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            int stageNum = i + 1;
            text.text = "Stage" + stageNum;
            button.GetComponent<StageButton>().LoadStageNum = i + 1;
            if (GamePlayManager.Instance.OpenedStageNum <= i)
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
	}

	void Update ()
	{
		
	}
}
