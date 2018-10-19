using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField,SceneName]
    string nextScene;
	
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
