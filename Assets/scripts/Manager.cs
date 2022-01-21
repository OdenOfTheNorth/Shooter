using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    [Header("Timer")]
    public float CurrentTime = 0;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI Level1Times;
    public List<TextMeshProUGUI> ListLevelTimes;
    //public TextMeshProUGUI[] ArrayLevelTimes;
    // Start is called before the first frame update

    private AudioListener audioListener;
    
    private void Awake()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        
        //PlayerPrefs.SetInt("Level_" + index + "_Completed", 0);
        
        if (PlayerPrefs.GetInt("Level_" + index + "_Completed") == 0)
        {
            PlayerPrefs.SetFloat("Level" + index, float.MaxValue);
        }
        
        FindAudioListener();
        CheckLevelTimers();
    }
    
    public void ResetTimer(int index)
    {
        PlayerPrefs.SetFloat("Level" + index, float.MaxValue);
        PlayerPrefs.SetInt("Level_" + index + "_Completed", 0);
    }

    public void CheckLevelTimers()
    {
        for (int i = 1; i < ListLevelTimes.Count; i++)
        {
            if (ListLevelTimes[i] == null)
            {
                print("List " + i);
                continue;
            }
            
            string level = "Level";
            float Time = PlayerPrefs.GetFloat(level + i);
            //print(Time);
            
            int hasPlayedLevel = PlayerPrefs.GetInt("Level_" + i + "_Completed");
            //print("Level_" + i + "_Completed" + hasPlayedLevel);
            if (hasPlayedLevel == 0)
            {
                PlayerPrefs.SetFloat("Level" + i, float.MaxValue);
                ListLevelTimes[i].text = "No Time";
            }
            else
            {
                ListLevelTimes[i].text = Time.ToString();
            }
        }
        
        //for (int i = 0; i < ArrayLevelTimes.Length; i++)
        //{
        //    if (ArrayLevelTimes[i] == null)
        //    {
        //        print("Array " + i);
        //        continue;
        //    }
        //    
        //    string level = "Level";
        //    ArrayLevelTimes[i].text = PlayerPrefs.GetFloat(level + i).ToString();
        //}
    }
    
    public void FindAudioListener()
    {
        audioListener = Camera.main.GetComponent<AudioListener>();
    }

    void Start()
    {
        CurrentTime = 0;

        if (!TimerText)
        {
            print("No Timer selected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckLevelTimers();

        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        if (audioListener)
        {
            FindAudioListener();
        }
        
        if (TimerText)
        {
            CurrentTime += Time.deltaTime;
            float f = CurrentTime;

            float truncated = (float)(Math.Truncate((double)f * 100) / 100f);

            float rounded = (float)(Math.Round((double)f, 2));

            float Truncate(float value, int digits)
            {
                double mult = Math.Pow(10.0, digits);
                double result = Math.Truncate( mult * value ) / mult;
                return (float) result;
            }
            
            TimerText.text = rounded.ToString();
        }
    }
    
    public void CompletedLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        string level = "Level";

        float BestTime = PlayerPrefs.GetFloat(level + index.ToString());
        print(BestTime);
        
        int hasPlayedLevel = PlayerPrefs.GetInt("Level_" + index + "_Completed");
        print("Level_" + index + "_Completed");
        
        if (hasPlayedLevel == 0)
        {
            print("hasPlayedLevel == false will turn true");
            PlayerPrefs.SetInt("Level_" + index + "_Completed", 1);
        }
        else
        {
            print("hasPlayedLevel == true");
        }
        
        if (CurrentTime < BestTime)
        {
            print("CurrentTime = " + CurrentTime+ " BestTime = " + BestTime);
            PlayerPrefs.SetFloat(level + index,CurrentTime); 
        }

        CurrentTime = 0;

        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        print("nextLevel before " + nextLevel + " sceneCount " + sceneCount);

        if (nextLevel >= sceneCount)
        {
            SceneManager.LoadScene("MenyLevel");
        }
        else
        {
            print("nextLevel after" + nextLevel);
            SceneManager.LoadScene(nextLevel);
        }
    }
    
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void SceneToLoad(string SceneLevel)
    {
        Time.timeScale = 1f;
        string level = "Level";
        float BestTime = PlayerPrefs.GetFloat(level + SceneManager.GetActiveScene().buildIndex.ToString());
        print("BestTime" + BestTime);
        
        print(SceneLevel);
        SceneManager.LoadScene(SceneLevel);
    }
    
    public void QuiteGame()
    {
        Application.Quit();
    }

    public void QuiteToMainMeny()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenyLevel");
    }
}

