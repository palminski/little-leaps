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
    private float inactiveAlpha = 0.1f;

    [SerializeField]
    private float pulseSpeed = 1;

    [SerializeField]
    private bool shouldPulse;

    [SerializeField]
    private RoomColor activeOnRoomColor;

    private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;
    [SerializeField] private RuleTile activeTile;
    [SerializeField] private RuleTile disabledTile;

    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material disabledMaterial;

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
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tileCollider = GetComponent<Collider2D>();
        if (tilemap) tilemap.color = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;

        deactiveColor = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.2f);

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
        if (activeTile && activeMaterial)
        {
            SetAllTiles(activeTile);
            tilemapRenderer.material = activeMaterial;
        }
    }

    private void Deactivate()
    {
        if (tilemap) tilemap.color = deactiveColor;
        StartCoroutine(WaitThenRemoveCollision());
        if (disabledTile && disabledMaterial)
        {
            SetAllTiles(disabledTile);
            tilemapRenderer.material = disabledMaterial;
        }
    }

    private void SetAllTiles(RuleTile tile)
    {
        foreach(var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilemap.SetTile(pos, tile);
            }
        }
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
        
        
        if(isActive)
        {
            baseColor.a -= alphaToSubtract;
        }
        else
        {
            baseColor.a = inactiveAlpha;
        }
        tilemap.color = baseColor;
    }
}
