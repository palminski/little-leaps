using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class ColorSwapper : MonoBehaviour
{
    private Color colorForRS0;

    private Color colorForRS1;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    private Image image;

    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        image = GetComponent<Image>();
        text = GetComponent<TMP_Text>();
        
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
            if (image) image.color = colorForRS0;
            if (text) text.color = colorForRS0;
            
        }
        else {
            if (spriteRenderer) spriteRenderer.color = colorForRS1;
            if (tilemap) tilemap.color = colorForRS1;
            if (image) image.color = colorForRS1;
            if (text) text.color = colorForRS1;
        }
    }

    private void HandleRoomStateChange() {
        UpdateColor();
    }

    
}
