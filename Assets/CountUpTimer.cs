using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountUpTimer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    public float timer = 0f;
    public GameObject border;
    public GameObject label;
    // public float interval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("ShowSpeedRunTimer"))
        {
            border.SetActive(false);
            label.SetActive(false);
            gameObject.SetActive(false);
            return;
        }
        timerText = GetComponent<TMP_Text>();
        timerText.text = ConvertFloatToTime(GameController.Instance.Timer);
    }

    // Update is called once per frame
    void Update()
    {
        
        timerText.text = ConvertFloatToTime(GameController.Instance.Timer);
            
    }

    private string ConvertFloatToTime(float currentTimer)
    {
        // if (currentTimer == 0) return "STABLE";
        // if (currentTimer < 0) return "CRITICAL";
        if (currentTimer > 5940) return "CRITICAL";
        int totalSeconds = (int) currentTimer;
        float minutes = Mathf.Floor(totalSeconds/60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }
}
