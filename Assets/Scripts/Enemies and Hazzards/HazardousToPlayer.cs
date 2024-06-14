using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//DEPTICATED???
public class HazardousToPlayer : MonoBehaviour
{
    [SerializeField]
    private int damage;


    private GameObject player;

    private Vector3 lastPosition;

    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastPosition = transform.position;
        
    }

    void LateUpdate() {
        lastPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            Player hitPlayer = player.GetComponent<Player>();

            if (!hitPlayer.IsInvincible())
            {
                int directionToShove = (player.transform.position.x > lastPosition.x) ? 1 : -1;
                hitPlayer.Damage(1, directionToShove);
                
                hitPlayer.Shove(directionToShove);
            }
        }
    }


}
