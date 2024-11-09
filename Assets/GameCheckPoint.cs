using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCheckPoint : MonoBehaviour
{
    public string room;
    


    [SerializeField] private TriggerEvent TriggerEvent;
    // Start is called before the first frame update
    void Start()
    {
        if (!TriggerEvent)
        {
            SaveDataManager.AddPermanentCollectedString(room);
            GameController.Instance.SetCheckPoint(room);
        }
    }

    private void OnEnable()
    {
        if (TriggerEvent)
        {
            TriggerEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (TriggerEvent)
        {
            TriggerEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    private void OnEventRaised() {
        SaveDataManager.AddPermanentCollectedString(room);
        GameController.Instance.SetCheckPoint(room);
    }
}
