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
    private bool startRight = true;

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
        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
        if (movementCollisionHandler.collisionInfo.right) direction = -1;
        if (movementCollisionHandler.collisionInfo.left) direction = 1;

        if (scaredOfHeights && IsAtEdge())
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

    private void TurnAround()
    {
        direction = -direction;
    }
}
