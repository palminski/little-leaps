using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardToPlayer : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    // void OnTriggerStay2D(Collider2D collision)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
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
}
