using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI TimerText;
    private Manager manager;
    private float currentTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<Manager>();
        TimerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        
        if (manager)
        {
            manager.CurrentTime = currentTime;
        }

        if (TimerText)
        {
            float f = currentTime;

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
}
