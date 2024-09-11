using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private string requiredKey;
    [SerializeField] private DialogueEvent ActivateEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    void OnEventRaised()
    {

        GameController.Instance.RemoveFollowingObject(requiredKey);
        GameController.Instance.TagObjectStringAsCollected(requiredKey);
    }
}
