using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusTimer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    public float timer = 0f;
    // public float interval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<TMP_Text>();
        timerText.text = ConvertFloatToTime(GameController.Instance.BonusTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
        timerText.text = ConvertFloatToTime(GameController.Instance.BonusTimer);
            
    }

    private string ConvertFloatToTime(float currentTimer)
    {
        if (currentTimer == 0) return "";
        if (currentTimer < 0) return "CRITICAL";
        int totalSeconds = (int) currentTimer;
        float minutes = Mathf.Floor(totalSeconds/60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }
}
