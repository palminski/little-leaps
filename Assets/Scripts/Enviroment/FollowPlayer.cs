using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowPlayer : MonoBehaviour
{
    private bool isFollowing = false;
    private MovementCollisionHandler movementCollisionHandler;
    private GameObject player;
    [SerializeField] private float magnetRadius = 1;
    [SerializeField] private float maxSpeed = 2;
    private Vector2 moveDirection = new(1,1);
    public float offset;
    private Vector3 targetPosition;
    private Vector3 lookForPlayerAt;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private Vector2 startDirection;

    private ShootAtPlayer shootAtPlayer;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        shootAtPlayer = GetComponent<ShootAtPlayer>();
        if (shootAtPlayer) shootAtPlayer.shouldShoot = false;
        player = GameObject.FindGameObjectWithTag("Player");
        moveDirection = startDirection.normalized;
    }

    public void AssignToPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.GetComponent<Player>().startPosition;
    }

    // Update is called once per frame
    void Update()
    {

        switch (isFollowing)
        {
            case true:
                LookForPlayer();
                break;
            case false:
                Idle();
                break;
        }

        if (movementCollisionHandler.InGround())
        {
            StartCoroutine(WaitCheckAndDamage());
        }

    }

    private void Idle() {
        Vector3 velocity = moveDirection * maxSpeed;
        

        Vector3 rayStartPosition = transform.position;
        
        Vector2 direction = velocity.normalized;
        // RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, maxSpeed * Time.deltaTime, movementCollisionHandler.collidableLayers);
        RaycastHit2D hit = Physics2D.BoxCast(
                    rayStartPosition,
                    (movementCollisionHandler.boxCollider.size) * 0.95f,
                    0f,
                    direction,
                    maxSpeed * Time.deltaTime + 0.05f,
                    movementCollisionHandler.collidableLayers
                );

        if (hit) 
        {
            moveDirection = Vector3.Reflect(velocity, hit.normal).normalized;
        }
        else
        {
            // transform.Translate(velocity * Time.deltaTime);
        movementCollisionHandler.Move((Vector3)(velocity * Time.deltaTime));

        }
        

        if (PlayerInRange()) {
            isFollowing = true;
            if (shootAtPlayer) shootAtPlayer.shouldShoot = true;
        } 
    }

    private bool PlayerInRange()
    {
        if (!player) return false;
        if (player && Vector3.Distance(transform.position, player.transform.position) > playerDetectionRadius) return false;

        Vector3 rayStartPosition = transform.position;
        float distance = Vector2.Distance(player.transform.position, rayStartPosition);
        Vector2 direction = (player.transform.position - rayStartPosition).normalized;

        // RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, direction, distance, movementCollisionHandler.collidableLayers);

        //check if we can get to the player from our current position
        RaycastHit2D hit = Physics2D.BoxCast(
                    transform.position,
                    (movementCollisionHandler.boxCollider.size) * 0.95f,
                    0f,
                    direction,
                    distance - 0.2f,
                    movementCollisionHandler.collidableLayers
                );
        if (hit)
        {
            return false;
        }

        return true;
    }

    private void LookForPlayer()
    {
        //new target position is the players position
        Vector3 newTargetPosition = targetPosition;
        if (player)
        {
            newTargetPosition = player.transform.position;
        }
        Vector2 newVectorToPlayer = newTargetPosition - transform.position;

        //check if we can get to the player from our current position
        RaycastHit2D hit = Physics2D.BoxCast(
                    transform.position,
                    (movementCollisionHandler.boxCollider.size) * 0.95f,
                    0f,
                    newVectorToPlayer,
                    newVectorToPlayer.magnitude - 0.2f,
                    movementCollisionHandler.collidableLayers
                );

        // If we can we set our actual target position to be the players position and respect the offset value
        bool shouldRespectOffset = false;
        if (hit.collider == null)
        {
            shouldRespectOffset = true;
            targetPosition = newTargetPosition;
        }
        //If we cannot reach the player we will continue towards our current target and calculate if we can get to the players next position from there

        Vector2 vectorToPlayerFromTarget = newTargetPosition - targetPosition;
        RaycastHit2D hitTwo = Physics2D.BoxCast(
                targetPosition,
                (movementCollisionHandler.boxCollider.size) * 0.95f,
                0f,
                vectorToPlayerFromTarget,
                vectorToPlayerFromTarget.magnitude - 0.2f,
                movementCollisionHandler.collidableLayers
            );
        if (hitTwo.collider == null)
        {
            lookForPlayerAt = newTargetPosition;
        }


        //if we are close to our target position go towards look for player at
        // if (Vector3.Distance(transform.position, targetPosition) < 0.2f && lookForPlayerAt != null && lookForPlayerAt != Vector3.negativeInfinity) {
        //     targetPosition = lookForPlayerAt;
        //     // lookForPlayerAt = Vector3.negativeInfinity;
        // }
        Vector2 vectorToLookForPlayerAtFromCurrent = lookForPlayerAt - transform.position;
        RaycastHit2D hitThree = Physics2D.BoxCast(
                transform.position,
                (movementCollisionHandler.boxCollider.size) * 0.95f,
                0f,
                vectorToLookForPlayerAtFromCurrent,
                vectorToLookForPlayerAtFromCurrent.magnitude - 0.2f,
                movementCollisionHandler.collidableLayers
            );
        if (hitThree.collider == null)
        {
            targetPosition = lookForPlayerAt;
        }

        //We calculate our target movement and move there
        Vector2 vectorToPlayer = targetPosition - transform.position;
        float distanceToPlayer = vectorToPlayer.magnitude - (shouldRespectOffset ? (1f * offset) : 0f);
        Vector2 stepMoveDirection = vectorToPlayer.normalized;
        float speed = shouldRespectOffset ? MathF.Min(maxSpeed, distanceToPlayer / magnetRadius * maxSpeed) : maxSpeed;
        
        if (player != null && Vector2.Distance(transform.position, lookForPlayerAt) < 0.2f || !player.activeSelf) {
            moveDirection = stepMoveDirection;
            isFollowing = false;
            if (shootAtPlayer) shootAtPlayer.shouldShoot = false;
        }

        movementCollisionHandler.Move((Vector3)(speed * Time.deltaTime * stepMoveDirection));
        
    }

    private IEnumerator WaitCheckAndDamage()
    {
        yield return new WaitForFixedUpdate();

        if (movementCollisionHandler.InGround())
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy) enemy.KillEnemy();
            StopAllCoroutines();
        }
    }

    void OnDrawGizmos()
    {
        // Draw detection range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.6f);

        // Draw detection range circle
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lookForPlayerAt, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Debug.DrawRay(transform.position, startDirection.normalized, Color.cyan);
    }
}
