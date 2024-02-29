using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileToggle : MonoBehaviour
{
    [SerializeField]
    private float maxAlphaSubtractionPercentage = 0.5f;
    [SerializeField]
    private float maxAlphaSubtractionPercentageInactive = 0.5f;

    [SerializeField]
    private float pulseSpeed = 1;

    [SerializeField]
    private bool shouldPulse;

    [SerializeField]
    private RoomColor activeOnRoomColor;

    private Tilemap tilemap;

    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    private Color deactiveColor;

    private Collider2D tileCollider;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileCollider = GetComponent<Collider2D>();
        if (tilemap) tilemap.color = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;

        deactiveColor = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.03f);

        HandleRoomStateChange();
    }

    void Update() {
        if (shouldPulse) {
            Pulse(activeOnRoomColor == GameController.Instance.RoomState);
        }
    }

    private void HandleRoomStateChange()
    {
        // UpdateColor();
        if (activeOnRoomColor == GameController.Instance.RoomState)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        if (tilemap) tilemap.color = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;
        if (tileCollider) tileCollider.enabled = true;
    }

    private void Deactivate()
    {
        if (tilemap) tilemap.color = deactiveColor;
        StartCoroutine(WaitThenRemoveCollision());
    }

    private IEnumerator WaitThenRemoveCollision() {
        yield return new WaitForSeconds(0.05f);
        if (tileCollider) tileCollider.enabled = false;
    }

    private void Pulse(bool isActive) {

        Color baseColor = deactiveColor;
        if (isActive) {
            baseColor = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;
        }

        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);

        // Interpolate alpha between minAlpha and maxAlpha
        float alphaToSubtract = isActive ? Mathf.Lerp(baseColor.a * maxAlphaSubtractionPercentage, 0, pulse) : Mathf.Lerp(baseColor.a * maxAlphaSubtractionPercentageInactive, 0, pulse);

        // Apply the modified alpha value to the tilemap color
        
        baseColor.a -= alphaToSubtract;
        tilemap.color = baseColor;
    }
}
