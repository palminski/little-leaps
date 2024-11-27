using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActions : MonoBehaviour
{

    public DialogueEvent Event;


    private void OnEnable()
    {
        Event.OnEventRaised.AddListener(OnEventRaised);
    }

    private void OnDisable()
    {
        Event.OnEventRaised.RemoveListener(OnEventRaised);
    }

    void OnEventRaised()
    {
        
        GameController.Instance.AddToScore(100);
    }
}
