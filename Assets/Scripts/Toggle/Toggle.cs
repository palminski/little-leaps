using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    public enum ToggleBehavior
    {
        OnRoomColor,
        OnKey,
    }
    public ToggleBehavior behavior = ToggleBehavior.OnRoomColor;
    [SerializeField]
    private GameObject objectForOn;

    [SerializeField]
    private GameObject objectForOff;

    [SerializeField]
    private bool toggleDelay;

    [SerializeField] private string requiredKey;


    private void OnEnable()
    {
        if (behavior == ToggleBehavior.OnRoomColor)
        {
            GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        }

    }
    private void OnDisable()
    {
        if (behavior == ToggleBehavior.OnRoomColor)
        {
            GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleChildren();
    }

    private void HandleRoomStateChange()
    {
        if (toggleDelay)
        {
            StartCoroutine(ToggleChildrenCoroutine());
        }
        else
        {
            ToggleChildren();
        }

    }

    public bool shouldToggle()
    {
        if (behavior == ToggleBehavior.OnRoomColor)
        {
            return GameController.Instance.RoomState == 0;
        }
        else if (behavior == ToggleBehavior.OnKey)
        {
            return GameController.Instance.FollowingObjects.Values.Any(obj => obj.Name ==requiredKey);
        }
        return false;
    }

    private void ToggleChildren()
    {
        if (shouldToggle())
        {
            if (objectForOn)
            {
                objectForOn.SetActive(true);
            }
            if (objectForOff)
            {
                objectForOff.SetActive(false);
            }
        }
        else
        {
            if (objectForOn)
            {
                objectForOn.SetActive(false);
            }
            if (objectForOff)
            {
                objectForOff.SetActive(true);
            }
        }
    }

    private IEnumerator ToggleChildrenCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        // yield return new WaitForFixedUpdate();
        // yield return new WaitForFixedUpdate();

        ToggleChildren();
    }
}
