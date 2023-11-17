using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    public Color colorForRS0;

    public Color colorForRS1;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSpriteColor();
    }

    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    private void UpdateSpriteColor() {
        if (GameController.Instance.RoomState == 0) {
            spriteRenderer.color = colorForRS0;
        }
        else {
            spriteRenderer.color = colorForRS1;
        }
    }

    private void HandleRoomStateChange() {
        UpdateSpriteColor();
    }

    
}
