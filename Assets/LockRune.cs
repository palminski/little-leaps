using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRune : MonoBehaviour
{
    [SerializeField] private string linkedChip;
    private ColorSwapper colorSwapper;

    [SerializeField] private TriggerEvent activateOnEvent;

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
    // Start is called before the first frame update
    void Awake()
    {
        colorSwapper = GetComponent<ColorSwapper>();
        if (!ShouldTurnOn())
        {
            if (colorSwapper) colorSwapper.enabled = false;
        }
    }

    bool ShouldTurnOn()
    {
        return GameController.Instance.SessionCollectedObjects.Contains(linkedChip);
    }

    private void OnEventRaised()
    {
        if (ShouldTurnOn()) colorSwapper.enabled = true;
    }
}
