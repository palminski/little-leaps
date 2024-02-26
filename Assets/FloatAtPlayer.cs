using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(MovementCollisionHandler))]
public class FloatAtPlayer : MonoBehaviour
{
    private MovementCollisionHandler movementCollisionHandler;
    private GameObject player;
    private Vector3 targetPoint;
    private bool hasLineOfSight = false;
    private enum EnemyState
    {
        Idle,
        Following
    }
    private EnemyState currentState;
    private Vector2 moveDirection = new(1,1);

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float playerDetectionRadius;
    [SerializeField]
    private Vector2 startDirection;
    


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();

        targetPoint = transform.position;
        currentState = EnemyState.Idle;
        moveDirection = startDirection.normalized; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
            break;
            case EnemyState.Following:
                FollowPlayer();
            break;
        }
       
    }

    private void Idle() {
        Vector3 velocity = moveDirection * moveSpeed;
        

        Vector3 rayStartPosition = transform.position;
        
        Vector2 direction = velocity.normalized;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, moveSpeed, movementCollisionHandler.collidableLayers);

        if (hit) 
        {
            moveDirection = Vector3.Reflect(velocity, hit.normal).normalized;
        }
        else
        {
            transform.Translate(velocity);
        }
        

        if (PlayerInRange()) currentState = EnemyState.Following;
    }

    private bool PlayerInRange()
    {
        
        if (Vector3.Distance(transform.position, player.transform.position) > playerDetectionRadius) return false;

        Vector3 rayStartPosition = transform.position;
        float distance = Vector2.Distance(player.transform.position, rayStartPosition);
        Vector2 direction = (player.transform.position - rayStartPosition).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, distance, movementCollisionHandler.collidableLayers);
        if (hit)
        {
            return false;
        }
        
        return true;
    }

    private void FollowPlayer() {
        UpdateTargetPoint();
        Vector3 additionToTarget = hasLineOfSight ? Vector3.zero : (targetPoint - transform.position).normalized * 3;
        
        // print(CheckForFlock());

        if (CanMoveToTargetPoint()) transform.Translate((targetPoint - transform.position + additionToTarget).normalized * moveSpeed);

        if (Vector3.Distance(transform.position, targetPoint) <= moveSpeed) currentState = EnemyState.Idle;
    }

    private void UpdateTargetPoint()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 rayStartPosition = transform.position;
        float distance = Vector2.Distance(playerPosition, rayStartPosition);
        Vector2 direction = (playerPosition - rayStartPosition).normalized;
        moveDirection = direction;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, distance, movementCollisionHandler.collidableLayers);
        if (!hit)
        {
            hasLineOfSight = true;
            targetPoint = playerPosition;
        }
        else
        {
            hasLineOfSight = false;
        }
    }

    private bool CanMoveToTargetPoint()
    {
        Vector3 rayStartPosition = transform.position;
        Vector2 direction = (targetPoint - rayStartPosition).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, 1, movementCollisionHandler.collidableLayers);
        if (hit)
        {
            currentState = EnemyState.Idle;
            return false;
        }
        return true;
    }

    private bool CheckForFlock() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2, LayerMask.GetMask("Entity"));
        foreach (Collider2D collider in colliders) {
            if (gameObject != collider.gameObject && collider.CompareTag("Flocker")) {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Debug.DrawRay(transform.position, startDirection.normalized, Color.cyan);
    }
    
}
