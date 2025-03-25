using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShiftCounter : MonoBehaviour
{
    private float fullHeight;
    [SerializeField] private float maxShifts;
    private float shiftCountdown;

    [SerializeField] private TriggerEvent eventToTrigger;
    [SerializeField] private TriggerEvent triggerEvent;

    // Start is called before the first frame update
    void Start()
    {
        fullHeight = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);

    }

    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.AddListener(Activate);
        }
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        if (triggerEvent)
        {
            triggerEvent.OnEventRaised.RemoveListener(Activate);
        }
    }

    private void Activate()
    {
        shiftCountdown = maxShifts;
        transform.localScale = new Vector3(transform.localScale.x, fullHeight, transform.localScale.z);

    }

    private void HandleRoomStateChange()
    {
        if (shiftCountdown <= 0) return;
        shiftCountdown--;
        if (shiftCountdown == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
            eventToTrigger.Raise();
            return;
        }
        transform.localScale = new Vector3(transform.localScale.x, fullHeight * (shiftCountdown / maxShifts), transform.localScale.z);
    }


}
