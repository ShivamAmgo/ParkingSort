using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
       // Vibration.Init();
        SceneManager.LoadScene(PlayerPrefs.GetInt("Level") >= SceneManager.sceneCountInBuildSettings
            ? PlayerPrefs.GetInt("ThisLevel")
            : PlayerPrefs.GetInt("Level", 1));
    }
}
