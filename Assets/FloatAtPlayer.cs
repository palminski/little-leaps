using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementCollisionHandler))]
public class FloatAtPlayer : MonoBehaviour
{
     private MovementCollisionHandler movementCollisionHandler;
    //  private Collider2D collider2d;

     private BoxCollider2D collider2d;
    private GameObject player;
    private Vector3 targetPoint;

    private bool hasLineOfSight = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        collider2d = GetComponent<BoxCollider2D>();
        targetPoint = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateTargetPoint();

         Vector3 additionToTarget = hasLineOfSight ? Vector3.zero : (targetPoint-transform.position).normalized * 5;

       if (CanMoveToTargetPoint()) transform.Translate((targetPoint-transform.position+ additionToTarget).normalized*0.05f); 
    }

    private void UpdateTargetPoint() {
        Vector3 playerPosition = player.transform.position;
        Vector3 rayStartPosition = transform.position;
        float distance = Vector2.Distance(playerPosition,rayStartPosition);
        Vector2 direction =  (playerPosition - rayStartPosition).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition,  direction, distance, movementCollisionHandler.collidableLayers);
        if (!hit) {
            hasLineOfSight = true;
            print($"has line of sight! {player.transform.position}");
            targetPoint = playerPosition;
        }
        else {
            hasLineOfSight = false;
            print ("cant see player...");
        }
        
    }

    private bool CanMoveToTargetPoint() {
        
        Vector3 rayStartPosition = transform.position;
        float distance = Vector2.Distance(targetPoint,rayStartPosition);
        Vector2 direction =  (targetPoint - rayStartPosition).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition,  direction, 1, movementCollisionHandler.collidableLayers);
        if (!hit) {
            return true;
        }
        return false;
    }

    // void OnDrawGizmos()
    // {
    //     Vector3 playerPosition = player.transform.position;
    //     Vector3 rayStartPosition = transform.position;
    //     float distance = Vector2.Distance(playerPosition,rayStartPosition);
    //     Vector2 direction =  (playerPosition - rayStartPosition).normalized;
    //     Debug.DrawRay(transform.position, direction * distance, Color.cyan);
    // }
}
