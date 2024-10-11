using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private bool shouldStartTimer;
    [SerializeField] private float bonusTimerValue = 0;
    [SerializeField] private string timerName = "";

    [SerializeField] private TriggerEvent activateOnEvent;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (activateOnEvent)
        {
            activateOnEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (activateOnEvent)
        {
            activateOnEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }
    void Start()
    {
        if (activateOnEvent != null) return;
        if (shouldStartTimer)
        {
            // if (GameController.Instance.CollectedObjects.Contains(timerName)) return;
            // GameController.Instance.startTimer(bonusTimerValue, timerName);
            // GameController.Instance.CollectedObjects.Add(timerName);
        }
        else
        {
            if (GameController.Instance.CollectedObjects.Contains(timerName)) return;
            GameController.Instance.CollectedObjects.Add(timerName);
            GameController.Instance.StopTimer();
        }
    }

    void OnEventRaised() {
        if (GameController.Instance.CollectedObjects.Contains(timerName)) return;
        GameController.Instance.CollectedObjects.Add(timerName);
        GameController.Instance.StopTimer();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
