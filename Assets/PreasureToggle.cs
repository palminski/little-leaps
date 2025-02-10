using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreasureToggle : MonoBehaviour
{

    private HazardToPlayer hazardToPlayer;
    private bool isActive = false;

    [SerializeField] private float timeToExtend = 0.5f;
    [SerializeField] private float timeToRetract = 1.5f;
    [SerializeField] private float timeToRetractOnPlainJump = 1.5f;


    private SpriteRenderer spriteRenderer;
    
    private Collider2D boxCollider;
    private Animator animator;

    private GameObject playerObject;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        hazardToPlayer = GetComponent<HazardToPlayer>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.PressedJump())
        {
            ExtendOnJump();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerObject && !isActive)
        {
            var colliderDistance = boxCollider.Distance(collision);
            if(Mathf.Abs(colliderDistance.distance) < 0.025f)
            {
                return;
            }
            StartCoroutine(ExtendAndRetractSpikes(timeToRetract));

        }
    }


    private void Activate()
    {
        StartCoroutine(WaitAndAddDanger());
        isActive = true;
        
        
    }

    private void Deactivate()
    {
        isActive = false;
        
        hazardToPlayer.SetCanDamagePlayer(false);
        
        if (animator)
        {
            animator.SetTrigger("Deactivate");
            return;
        }
    }

    private IEnumerator ExtendAndRetractSpikes(float timeToComplete)
    {
        
        Activate();
        yield return new WaitForSeconds(timeToComplete);
        Deactivate();
    }

    private IEnumerator WaitAndAddDanger()
    {
        yield return new WaitForSeconds(timeToExtend);
        
        if (animator) animator.SetTrigger("Activate");
        hazardToPlayer.SetCanDamagePlayer(true);
    }

    private void ExtendOnJump()
    {
        
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;
        LayerMask collidableLayers = LayerMask.GetMask("Entity");

        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);

        if (overlapCollider != null) {
            Player playerComponent = overlapCollider.GetComponent<Player>();
            if (playerComponent)
            {
                StopAllCoroutines();
                StartCoroutine(ExtendAndRetractSpikes(timeToRetractOnPlainJump));
            }
        } 
    }
}
