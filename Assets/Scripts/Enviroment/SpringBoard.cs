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

    [SerializeField] private float maxActiveDuration = 0.25f;
    private float activeDuration;

    private Animator animator;
    public Animator veinsAnimator;

    public SpriteRenderer sr;

    // Start is called before the first frame update
    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += SpringPlayer;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= SpringPlayer;
    }
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        boxCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeDuration > 0) activeDuration -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (!CanBounce()) return;
        if (activeDuration > 0)
        {
            Player playerComponent = hitCollider.GetComponent<Player>();
            if (playerComponent)
            {
                if (sr.isVisible) if (AudioController.Instance != null) AudioController.Instance.PlaySpringNoise();
                // playerComponent.transform.Translate(new Vector3(0,2f,0));
                activeDuration = 0;
                StartCoroutine(BounceAtEndOfFrame(playerComponent));
            }
        }
    }

    void SpringPlayer()
    {
        activeDuration = maxActiveDuration;
        if (!CanBounce()) return;
        
        if (animator) animator.SetTrigger("Launch");
        if (veinsAnimator) veinsAnimator.SetTrigger("Launch");
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;

        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter, boxSize, 0, collidableLayers);
        if (overlapCollider != null)
        {
            Player playerComponent = overlapCollider.GetComponent<Player>();
            if (playerComponent)
            {
                if (sr.isVisible) if (AudioController.Instance != null) AudioController.Instance.PlaySpringNoise();
                activeDuration = 0;
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

    private IEnumerator BounceAtEndOfFrame(Player playerComponent)
    {
        yield return new WaitForEndOfFrame();
        playerComponent.Bounce(bounceMultiplier);
    }
}
