using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    private GameObject player;

        [SerializeField]
    private GameObject blood;

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
            print($"hit play at {hitCollider.bounds.center.y}. this collision box center: {collider.bounds.center.y} ");

            if (playerBottom > enemyTop)
            {
                hitPlayer.ResetCoyoteTime();
                hitPlayer.Bounce();
                GameController.Instance.PullFromPool(blood,transform.position);
                Destroy(transform.parent.gameObject);
                return;
            }
        }
    }
}
