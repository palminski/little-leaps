using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectToggle : MonoBehaviour
{

    [SerializeField]
    private RoomColor activeOnRoomColor;
    [SerializeField]
    private float deactiveAlpha = 0.1f;
    [SerializeField]
    private Sprite deactiveSprite;

    private SpriteRenderer spriteRenderer;
    private Sprite activeSprite;
    private Color activeColor;
    private Color deactiveColor;
    private Collider2D boxCollider;

    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<Collider2D>();

        activeColor = spriteRenderer.color;
        activeSprite = spriteRenderer.sprite;
        deactiveColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, deactiveAlpha);

        HandleRoomStateChange();
    }

    private void HandleRoomStateChange()
    {
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
        if (spriteRenderer) spriteRenderer.color = activeColor;
        if (spriteRenderer) spriteRenderer.sprite = activeSprite;

        if (boxCollider) boxCollider.enabled = true;
    }

    private void Deactivate()
    {
        StartCoroutine(WaitThenRemoveCollision());

        if (spriteRenderer && deactiveSprite){
            spriteRenderer.sprite = deactiveSprite;
            return;
        }

        if (spriteRenderer) spriteRenderer.color = deactiveColor;
    }

    private IEnumerator WaitThenRemoveCollision() {
        yield return new WaitForSeconds(0.05f);
        if (boxCollider) boxCollider.enabled = false;
    }
}
