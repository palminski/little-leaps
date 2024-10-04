using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusTimer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    public float timer = 0f;
    public float interval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<TMP_Text>();
        timerText.text = ConvertFloatToTime(GameController.Instance.BonusTimer);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timerText.text = ConvertFloatToTime(GameController.Instance.BonusTimer);
            timer = 0f;
        }
    }

    private string ConvertFloatToTime(float currentTimer)
    {
        int timer = (int) currentTimer;
        float minutes = Mathf.Floor(timer/60);
        int seconds = timer % 60;
        return minutes + ":" + seconds.ToString("D2");
    }
}
