using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCheckPoint : MonoBehaviour
{
    public string key;
    


    [SerializeField] private TriggerEvent TriggerEvent;
    // Start is called before the first frame update
    void Start()
    {
        if (!TriggerEvent && key.Length > 0)
        {
            SaveDataManager.AddPermanentCollectedString(key);
            // GameController.Instance.SetCheckPoint(key);
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
        if (key.Length > 0)
        {
            SaveDataManager.AddPermanentCollectedString(key);
        }
        // GameController.Instance.SetCheckPoint(SceneManager.GetActiveScene().name);
    }
}
