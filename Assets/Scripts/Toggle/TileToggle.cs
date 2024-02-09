using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileToggle : MonoBehaviour
{

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
}
