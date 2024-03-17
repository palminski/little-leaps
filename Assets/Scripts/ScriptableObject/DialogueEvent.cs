using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Dialogue/DialogueEvent")]
public class DialogueEvent : ScriptableObject
{
    public UnityEvent OnEventRaised;

    public void Raise()
    {
        OnEventRaised.Invoke();
    }
}
