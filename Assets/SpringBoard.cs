using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : MonoBehaviour
{
    public enum ActiveColor
    {
        Both,
        Purple,
        Green,
    }
    private GameObject player;
    private Collider2D boxCollider;
    [SerializeField] public LayerMask collidableLayers;
    [SerializeField] public float bounceMultiplier = 1.5f;
    [SerializeField] public ActiveColor activeColor = ActiveColor.Both;

    private Animator animator;
    public Animator veinsAnimator;
    
    // Start is called before the first frame update
    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += SpringPlayer;
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= SpringPlayer;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boxCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpringPlayer()
    {
        if (!CanBounce()) return;
        if (animator) animator.SetTrigger("Launch");
        if (veinsAnimator) veinsAnimator.SetTrigger("Launch");
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;
        
        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);
        if (overlapCollider != null) {
            Player playerComponent = overlapCollider.GetComponent<Player>();
            if (playerComponent)
            {
                playerComponent.Bounce(bounceMultiplier);
            }
        } 
    }

    private bool CanBounce()
    {
        return (
                activeColor == ActiveColor.Both || 
                (activeColor == ActiveColor.Purple && GameController.Instance.RoomState == RoomColor.Purple) || 
                (activeColor == ActiveColor.Green && GameController.Instance.RoomState == RoomColor.Green)
            );
    }
}
