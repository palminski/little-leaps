using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Collider2D boxCollider;

    private Player player;
    
    void Awake()
    {
        //Maybe do this better, or at least dont rely on attack being the child of the player
        player = transform.parent.gameObject.GetComponent<Player>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = transform.parent.localScale;
    }

    public void HideAttack()
    {
        gameObject.SetActive(false);
    }

    // void OnTriggerEnter2D(Collider2D hitCollider)
    // {
        
        
        
    //     Enemy hitEnemy = hitCollider.gameObject.GetComponent<Enemy>();
    //     if (hitEnemy)
    //     {
    //         Collider2D playerCollider = transform.parent.gameObject.GetComponent<Collider2D>();
    //         float playerBottom = player.GetLastPosition().y + playerCollider.offset.y  - (playerCollider.bounds.size.y / 2);
    //         if (playerBottom > hitCollider.transform.position.y && player.IsInAir() && IsAttackingDown())
    //         {
                
    //             player.Bounce();
    //         }
    //         hitEnemy.DamageEnemy();
    //     }
    // }

    public bool IsAttackingDown()
    {
        Vector3 direction = transform.rotation * Vector3.down;
        return Mathf.Approximately(direction.x ,-1f);
    }
}
