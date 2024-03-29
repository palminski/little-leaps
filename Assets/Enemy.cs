using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private int pointValue = 100;

    [SerializeField]
    private GameObject blood;


    private GameObject player;

    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
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
            if (playerBottom > enemyTop)
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
    }


    public void KillEnemy() {
        GameController.Instance.AddToScore(pointValue);
        Destroy(gameObject);
    }


}
