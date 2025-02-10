using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    private GameObject player;
    private GameObject playerAttack;

    [SerializeField]
    private GameObject blood;

    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = GameObject.FindGameObjectWithTag("PlayerAttack");
        enemy = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
        
            
            // Player hitPlayer = player.GetComponent<Player>();
            // Collider2D collider;
            // collider = GetComponent<Collider2D>();

            // float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y  - (hitCollider.bounds.size.y / 2);
            // float enemyTop = transform.position.y + collider.offset.y + collider.bounds.size.y / 2;
            

            // if (playerBottom > enemyTop && hitPlayer.IsDashing())
            // {
            //     hitPlayer.ResetCoyoteTime();
            //     hitPlayer.Bounce();
            //     GameController.Instance.PullFromPool(blood,transform.position);

            //     if (enemy) {
            //         enemy.KillEnemy();
            //     }
            //     else {
            //         Destroy(transform.parent.gameObject);
            //     }
            //     return;
            // }
        }
    }
}
