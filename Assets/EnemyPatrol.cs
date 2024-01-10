using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementCollisionHandler))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.35f;

    [SerializeField]
    private bool scaredOfHeights = false;

    [SerializeField]
    private bool canHop = false;

    [SerializeField]
    private float hopDetectionDist = 1;
    [SerializeField]
    private float hopDetectionHeight = 1;



    [SerializeField]
    private bool startRight = true;

    [SerializeField]
    private float jumpPower = 1f;

    [SerializeField]
    private float gravity = 0.08f;

    private MovementCollisionHandler movementCollisionHandler;
    private Collider2D collider2d;
    private int direction;
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        collider2d = GetComponent<Collider2D>();
        direction = startRight ? 1 : -1;
    }

    void FixedUpdate()
    {


        velocity.x = direction * speed;

        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) velocity.y = 0;

        velocity.y -= gravity;

    if (canHop && ShouldJump() && movementCollisionHandler.collisionInfo.below)
        {
            print("foo");
            velocity.y = jumpPower;
        }

        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
        if (movementCollisionHandler.collisionInfo.right) direction = -1;
        if (movementCollisionHandler.collisionInfo.left) direction = 1;

        if (scaredOfHeights && IsAtEdge() && movementCollisionHandler.collisionInfo.below)
        {
            TurnAround();
        }
        


    }

    // Update is called once per frame
    void Update()
    {

        if (direction != 0)
        {
            Vector3 newScale = new(direction, 1, 1);
            transform.localScale = newScale;
        }
    }

    private bool IsAtEdge()
    {
        Vector2 rayStartPosition = collider2d.bounds.center;
        rayStartPosition.x += direction * collider2d.bounds.extents.x;
        rayStartPosition.y -= collider2d.bounds.extents.y;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPosition, Vector2.down, 0.5f, LayerMask.GetMask("Solid"));
        return hit.collider == null;
    }

    private bool ShouldJump()
    {
        Vector2 rayStartPosition = collider2d.bounds.center;
        rayStartPosition.x += direction * collider2d.bounds.extents.x;
        rayStartPosition.y -= collider2d.bounds.extents.y-0.005f;


        Debug.DrawRay(rayStartPosition, new Vector3(hopDetectionDist * direction, 0, 0), Color.red);
        RaycastHit2D lowerHit = Physics2D.Raycast(rayStartPosition, new Vector2(direction, 0), hopDetectionDist, LayerMask.GetMask("Solid"));
        rayStartPosition.y += hopDetectionHeight;
        Debug.DrawRay(rayStartPosition, new Vector3(hopDetectionDist * direction, 0, 0), Color.red);
        RaycastHit2D upperHit = Physics2D.Raycast(rayStartPosition, new Vector2(direction, 0), hopDetectionDist, LayerMask.GetMask("Solid"));

        if (lowerHit.collider != null && upperHit.collider == null) return true;
        return false;
    }

    private void TurnAround()
    {
        direction = -direction;
    }
}
