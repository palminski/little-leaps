using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TriggerEvent")]
public class TriggerEvent : ScriptableObject
{
    public UnityEvent OnEventRaised;

    public void Raise()
    {
        OnEventRaised.Invoke();
    }
}
