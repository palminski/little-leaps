using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int pointValue = 100;

    [SerializeField]
    private int health = 1;

    [SerializeField]
    private GameObject blood;


    private GameObject player;
    private GameObject playerAttack;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = GameObject.FindGameObjectWithTag("PlayerAttack");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            
            Player hitPlayer = player.GetComponent<Player>();

            Collider2D collider;
            collider = GetComponent<Collider2D>();

            float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y  - (hitCollider.bounds.size.y / 2);
            float enemyTop = transform.position.y + collider.offset.y + collider.bounds.size.y / 2;
            if (playerBottom > enemyTop && hitPlayer.IsDashing())
            {
                
                return;
            }

            if (!hitPlayer.IsInvincible())
            {
                

                int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
                GameController.Instance.ChangeHealth(-1);
                hitPlayer.Shove(directionToShove);
            }
        }
        if (hitCollider.gameObject == playerAttack)
        {
            
            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider;
            collider = GetComponent<Collider2D>();

            float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y  - (hitCollider.bounds.size.y / 2);
            float enemyTop = transform.position.y + collider.offset.y + collider.bounds.size.y / 2;
            

            if (playerBottom > enemyTop && hitPlayer.IsDashing())
            {
                hitPlayer.ResetCoyoteTime();
                hitPlayer.Bounce();
                GameController.Instance.PullFromPool(blood,transform.position);

                KillEnemy();
                
            }
        }
    }
    

    public void DamageEnemy(int damage=1) {
        health -= damage;
        if (health <= 0)
        {
            KillEnemy();
        }
    }

    public void KillEnemy() {
        // GameController.Instance.AddToScore(pointValue);
        if (blood) GameController.Instance.PullFromPool(blood,transform.position);
        GameLight light = GetComponentInChildren<GameLight>();
        
        if (light) {
            light.transform.SetParent(null);
            light.Fade();
        }
        Destroy(gameObject);
    }


}
