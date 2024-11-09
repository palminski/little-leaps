using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerSpawnPoint : MonoBehaviour
{
    private Player player;

    [SerializeField] private Transform newSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }


    // void OnTriggerStay2D(Collider2D collision)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (player && collision.gameObject == player.gameObject)
        {
            player.SetPlayerSpawnPointToTransform(newSpawnPoint);
        }
    }
}
