using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private GameObject blood;


    private GameObject player;

    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {

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
                //Eventually expand this to add points, some blood effects, etc
                //Probably move it to its own method 
                print("Killed Enemy!");
                hitPlayer.ResetCoyoteTime();
                hitPlayer.Bounce();
                GameObject.Instantiate(blood, transform.position, transform.rotation);
                Destroy(gameObject);
                return;
            }

            if (!hitPlayer.IsInvincible())
            {
                print("Hit Player!");
                int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
                hitPlayer.Shove(directionToShove);
            }
        }
    }


}
