using System;
using TMPro;
using UnityEngine;

// Uses TextMeshPro Text
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
        // Check if it's a countdown - if it is, decrease currentTime by the time that has passed. If it's not a countdown, add the time that has passed.
        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
        
        // Check if there's a limit on the clock, if there is check and see if that limit has been reached
        if (hasLimit && ((countDown && currentTime <= timerLimit) || (!countDown && currentTime >= timerLimit))){
            // Lock the time at the limit and stop updating the time
            currentTime = timerLimit;
            SetTimerText();
            timerText.color = Color.red;
            enabled = false;
        }
        SetTimerText();

    }

    private void SetTimerText()
    {
        // Convert currentTime (seconds) to minutes by flooring (gets the integer quotient of currentTime divided by 60)
        double minutes = Math.Floor(currentTime/60);
        
        // Get the modulus (remainder) of the currentTime divided by 60
        double seconds = currentTime % 60;
        
        // Convert the minutes & seconds to strings with a format of MM:SS
        string time = $"{minutes.ToString("00")}:{seconds.ToString("00")}";

        timerText.text = time;
    } 
}
