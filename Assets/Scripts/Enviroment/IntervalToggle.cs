using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IntervalToggle : MonoBehaviour
{
    [SerializeField] private float interval = 1;
    [SerializeField] private bool startActive = false;
    private float swapCounter;
    private bool active = false;

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

    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        activeColor = spriteRenderer.color;
        activeSprite = spriteRenderer.sprite;
        deactiveColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, deactiveAlpha);

        //This looks dumb, but Swap() will immediatly switch it to its opposite
        if (!startActive) active = true;
        Swap();
    }

    void Update()
    {
        swapCounter += Time.deltaTime;
        if (swapCounter >= interval)
        {
            swapCounter = 0;
            Swap();
        }
    }

    private void Swap()
    {
        if (active)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    private void Activate()
    {
        active = true;
        if (AudioController.Instance != null) AudioController.Instance.PlayBlueBlokcs();
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
        active = false;
        if (shouldRemoveCollision) StartCoroutine(WaitThenRemoveCollision());
        if (animator)
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
}
