using System;
using TMPro;
using UnityEngine;

public class TimerStopwatchScript : MonoBehaviour
{
    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    public bool countDown;
    public float currentTime;

    [Header("Timer Limits")]
    public bool hasLimit;
    public float timerLimit;

    private void Update()
    {
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;

        if (hasLimit && ((countDown && currentTime <= timerLimit) || (!countDown && currentTime >= timerLimit))){
            currentTime = timerLimit;
            SetTimerText();
            timerText.color = Color.red;
            enabled = false;
        }
        SetTimerText();

    }

    private void SetTimerText()
    {
        double minutes = Math.Floor(currentTime/60);
        double seconds = currentTime % 60;
        string time = $"{minutes.ToString("00")}:{seconds.ToString("00")}";

        timerText.text = time;
    } 
}
