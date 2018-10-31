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
    [SerializeField]
    int maxStage = 8;

	void Start ()
	{
        SoundManager.Instance.PlayBgmSingle(bgm);
        for(int i = 0; i < 8; i++)
        {
            var button = Instantiate(stageButtonPrefab, stageButtons);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            int stageNum = i + 1;
            text.text = "Stage" + stageNum;
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
