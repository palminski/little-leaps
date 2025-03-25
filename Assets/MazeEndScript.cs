using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEndScript : MonoBehaviour
{
    [SerializeField] private TriggerEvent triggerEvent;
    [SerializeField] private TriggerEvent triggerEventNegative;

    private void OnEnable()
    {
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.AddListener(RaisePrestige);
        }

        if (triggerEventNegative)
        {
            triggerEventNegative.OnEventRaised.AddListener(JustProceed);
        }
    }
    private void OnDisable()
    {
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.RemoveListener(RaisePrestige);
        }

        if (triggerEventNegative)
        {
            triggerEventNegative.OnEventRaised.RemoveListener(JustProceed);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.StopTimer();
        SaveDataManager.AddPermanentCollectedString("Secret Area Reached");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RaisePrestige()
    {
        int newMaxHealth = GlobalConstants.lowestAllowedMaxHealth;
        int newPrestige = GlobalConstants.highestAllowedPrestige;

        GameController.Instance.UpdateMultiplier(newPrestige, GameController.Instance.MaxHealth, GameController.Instance.SessionHealing);
        GameController.Instance.UpdateMultiplier(GameController.Instance.SessionPrestige, newMaxHealth, GameController.Instance.SessionHealing);
        GameController.Instance.UpdateMultiplier(GameController.Instance.SessionPrestige, GameController.Instance.MaxHealth, 10f);


        float timeSet = GlobalConstants.prestigeTime[GameController.Instance.SessionPrestige];
        GameController.Instance.StartTimer(timeSet);
        GameController.Instance.SetStartingTimer(timeSet);

        if (GameController.Instance.CheckpointBackend != "" && GameController.Instance.CheckpointBackend != "Main Menu")
        {
            GameController.Instance.ChangeScene(GameController.Instance.CheckpointBackend);
        }
        else
        {
            GameController.Instance.SetCheckPointBackend("backend_start");
            GameController.Instance.ChangeScene("backend_start");
        }
    }

    private void JustProceed()
    {
        float timeSet = GlobalConstants.prestigeTime[GameController.Instance.SessionPrestige];
        GameController.Instance.StartTimer(timeSet);
        GameController.Instance.SetStartingTimer(timeSet);
        if (GameController.Instance.CheckpointBackend != "")
        {
            GameController.Instance.ChangeScene(GameController.Instance.CheckpointBackend);
        }
        else
        {
            GameController.Instance.ChangeScene("backend_start");
        }
    }
}
