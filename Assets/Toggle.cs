using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField]
    private GameObject objectForState0;

    [SerializeField]
    private GameObject objectForState1;


    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleChildren();
    }

    private void HandleRoomStateChange() {
        ToggleChildren();
    }

    private void ToggleChildren() {
        if (GameController.Instance.RoomState == 0) {
            if (objectForState0) {
                objectForState0.SetActive(true);
            }
            if (objectForState1) {
                objectForState1.SetActive(false);
            }
        }
        else {
            if (objectForState0) {
                objectForState0.SetActive(false);
            }
            if (objectForState1) {
                objectForState1.SetActive(true);
            }
        }
    }
}
