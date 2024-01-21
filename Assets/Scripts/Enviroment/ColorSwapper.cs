using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorSwapper : MonoBehaviour
{
    private Color colorForRS0;

    private Color colorForRS1;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        
        colorForRS0 = GameController.ColorForPurple;
        colorForRS1 = GameController.ColorForGreen;

        UpdateColor();
    }

    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        UpdateColor();
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    private void UpdateColor() {
        if (GameController.Instance.RoomState == RoomColor.Purple) {
            if (spriteRenderer) spriteRenderer.color = colorForRS0;
            if (tilemap) tilemap.color = colorForRS0;
            
        }
        else {
            if (spriteRenderer) spriteRenderer.color = colorForRS1;
            if (tilemap) tilemap.color = colorForRS1;
        }
    }

    private void HandleRoomStateChange() {
        UpdateColor();
    }

    
}
