using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimes : MonoBehaviour
{
    public int timeIndex = -1;
    public TextMeshProUGUI TimerText;
    public Manager manager;
    void Awake()
    {
        manager = FindObjectOfType<Manager>();
        TimerText = GetComponent<TextMeshProUGUI>();
    }
/*
    // Update is called once per frame
    void Update()
    {
        if (manager)
        {
            if (manager.ListLevelTimes.Length > timeIndex)
            {
                manager.ListLevelTimes[timeIndex] = TimerText;
            }
            else
            {
                //Debug.Break();
                print("manager Count is " + manager.ListLevelTimes.Length + " timeIndex = " + timeIndex);
            }
        }
        else
        {
            print("Did not find manager");
        }
    }*/
}
