using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardToPlayer : MonoBehaviour
{
    private GameObject player;

    private bool canDamagePlayer = true;
    [SerializeField] public LayerMask collidableLayers;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // void OnTriggerStay2D(Collider2D collision)
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player && canDamagePlayer)
        {
           
            Player hitPlayer = player.GetComponent<Player>();

            if (!hitPlayer.IsInvincible())
            {
                int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
                hitPlayer.Damage(1, directionToShove);
                // hitPlayer.Shove(directionToShove);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player && canDamagePlayer)
        {
            
            Player hitPlayer = player.GetComponent<Player>();

            if (!hitPlayer.IsInvincible())
            {
                int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
                hitPlayer.Damage(1, directionToShove);
                // hitPlayer.Shove(directionToShove);
            }
        }
    }

    public void SetCanDamagePlayer(bool shouldDamagePlayer)
    {
        canDamagePlayer = shouldDamagePlayer;
        Collider2D boxCollider = GetComponent<Collider2D>();
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;
        
        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);
        if (overlapCollider != null) {
            Player playerComponent = overlapCollider.GetComponent<Player>();
            if (playerComponent)
            {
                playerComponent.Damage(1, 0);
            }
        } 
    }
}
