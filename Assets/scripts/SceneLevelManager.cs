using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLevelManager : MonoBehaviour
{
    public void CompletedLevel(float time)
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        string level = "Level" + index;

        float BestTime = PlayerPrefs.GetFloat(level);
        
        if (time < BestTime)
        {
            PlayerPrefs.SetFloat(level,time); 
        }
    }
    
    public void SceneToLoad(string SceneLevel)
    {
        print(SceneLevel);
        SceneManager.LoadScene(SceneLevel);
    }
    
    public void QuiteGame()
    {
        Application.Quit();
    }
}


