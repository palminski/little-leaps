using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private bool shouldStartTimer;
    [SerializeField] private float bonusTimerValue = 0;
    [SerializeField] private string timerName = "";
    // Start is called before the first frame update
    void Start()
    {
        if (shouldStartTimer)
        {
            if (GameController.Instance.CollectedObjects.Contains(timerName)) return;
            GameController.Instance.startTimer(bonusTimerValue, timerName);
            GameController.Instance.CollectedObjects.Add(timerName);
        }
        else
        {
            if (GameController.Instance.CurrentTimer != timerName) return;
            GameController.Instance.stopTimer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
