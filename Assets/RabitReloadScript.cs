using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabitReloadScript : MonoBehaviour
{
    [SerializeField] private TriggerEvent triggerEvent;
    [SerializeField] private TriggerEvent triggerEventNegative;

    private void OnEnable()
    {
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.AddListener(ReloadRabit);
        }

        if (triggerEventNegative)
        {
            triggerEventNegative.OnEventRaised.AddListener(ReturnToMainMenu);
        }
    }
    private void OnDisable()
    {
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.RemoveListener(ReloadRabit);
        }

        if (triggerEventNegative)
        {
            triggerEventNegative.OnEventRaised.RemoveListener(ReturnToMainMenu);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.StopTimer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ReloadRabit()
    {
        SaveDataManager.RemovePermanentCollectedString("RABIT escaped");
        GameController.Instance.ChangeScene((GameController.Instance.Checkpoint != null && GameController.Instance.Checkpoint.Length > 0) ? GameController.Instance.Checkpoint : "Main Menu");
        GameController.Instance.SetCheckPoint("Main Menu");
    }

    private void ReturnToMainMenu()
    {
        GameController.Instance.ChangeScene("Main Menu");

    }
}
