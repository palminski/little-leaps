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
    [SerializeField]
    private bool shouldRemoveCollision = true;

    public bool shouldWaitBeforeAddingCollision = false;

    private SpriteRenderer spriteRenderer;
    private Sprite activeSprite;
    private Color activeColor;

    
    private Color deactiveColor;
    private Collider2D boxCollider;
    private Animator animator;


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
        animator = GetComponent<Animator>();

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
        if (animator) animator.SetTrigger("Activate");
        if (shouldWaitBeforeAddingCollision)
        {
            StartCoroutine(WaitThenAddCollision());
            return;
        }
        if (boxCollider) boxCollider.enabled = true;
    }

    private void Deactivate()
    {
        if (shouldRemoveCollision) StartCoroutine(WaitThenRemoveCollision());
        if (animator && AnimatorHasParameter("Deactivate", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger("Deactivate");
            return;
        }
        if (spriteRenderer && deactiveSprite)
        {
            spriteRenderer.sprite = deactiveSprite;
            return;
        }

        if (spriteRenderer) spriteRenderer.color = deactiveColor;
        if (spriteRenderer) print("here");
    }

    private IEnumerator WaitThenRemoveCollision()
    {
        yield return new WaitForSeconds(0.05f);
        if (boxCollider) boxCollider.enabled = false;
    }

    private IEnumerator WaitThenAddCollision()
    {
        yield return new WaitForSeconds(0.05f);
        if (boxCollider) boxCollider.enabled = true;
    }

    private bool AnimatorHasParameter(string parameter, AnimatorControllerParameterType parameterType)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameter && param.type == parameterType) return true;
        }
        return false;
    }
}
