using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage;


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

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject == player) {
            print("Hit Player!");
            int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
            player.GetComponent<Player>().Shove(directionToShove);
        }
    }
}
