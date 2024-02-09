using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectToggle : MonoBehaviour
{

    [SerializeField]
    private RoomColor activeOnRoomColor;

    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileCollider = GetComponent<Collider2D>();
        if (spriteRenderer) spriteRenderer.color = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;

        deactiveColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.03f);

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
        if (spriteRenderer) spriteRenderer.color = activeOnRoomColor == RoomColor.Purple ? GameController.ColorForPurple : GameController.ColorForGreen;
        if (tileCollider) tileCollider.enabled = true;
    }

    private void Deactivate()
    {
        if (spriteRenderer) spriteRenderer.color = deactiveColor;
        StartCoroutine(WaitThenRemoveCollision());
    }

    private IEnumerator WaitThenRemoveCollision() {
        yield return new WaitForSeconds(0.05f);
        if (tileCollider) tileCollider.enabled = false;
    }
}
