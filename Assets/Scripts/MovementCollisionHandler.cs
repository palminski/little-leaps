using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementCollisionHandler : MonoBehaviour
{
    //Define
    //====================================================================================================================
    private BoxCollider2D boxCollider;

    // [SerializeField]
    // private Vector3 velocity;

    [SerializeField]
    private int xRayCount = 4;

    [SerializeField]
    private int yRayCount = 4;

    private float xRaySpacing;

    private float yRaySpacing;

    private RaycastOrigins raycastOrigins;

    public CollisionInfo collisionInfo;

    //====================================================================================================================
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        updateRaySpacing();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {

    }

    public void Move(Vector3 velocity)
    {
        collisionInfo.Reset();

        updateRaycastOrigins();
        if (velocity.x != 0) CollisionsXAxis(ref velocity);
        if (velocity.y != 0) CollisionsYAxis(ref velocity);


        transform.Translate(velocity);
    }

    void CollisionsXAxis(ref Vector3 velocity)
    {
        //Define some useful variables
        float xVelocity = velocity.x;
        if (xVelocity == 0) return;
        float direction = Mathf.Sign(xVelocity);
        float distanceToCast = Mathf.Abs(xVelocity);

        Vector2 rayOrigin;
        Vector2 rayDirection = new Vector2(direction, 0);

        //Determine if moving left or right and decide where to start ray iteration
        if (direction > 0)
        {
            rayOrigin = raycastOrigins.bottomRight;
        }
        else
        {
            rayOrigin = raycastOrigins.bottomLeft;
        }

        //For loop runs however many times we have set rays to be cast in horizontal direction
        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * xRaySpacing * Vector2.up, rayDirection, distanceToCast, LayerMask.GetMask("Solid"));
            //If there is a collision we will update velocity.x accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                velocity.x = (collision.distance - 0.01f) * direction;
                distanceToCast = collision.distance;
                
                collisionInfo.left = direction == -1;
                collisionInfo.right = direction == 1;
            }
        }

    }

    void CollisionsYAxis(ref Vector3 velocity)
    {
        float yVelocity = velocity.y;
        if (yVelocity == 0) return;
        float direction = Mathf.Sign(yVelocity);
        float distanceToCast = Mathf.Abs(yVelocity);

        Vector2 rayOrigin;
        Vector2 rayDirection = new Vector2(0, direction);

        //Determine if moving up or down and decide where to start ray iteration
        if (direction > 0)
        {
            rayOrigin = raycastOrigins.topLeft;
        }
        else
        {
            rayOrigin = raycastOrigins.bottomLeft;
        }

        //For loop runs however many times we have set rays to be cast in vertical direction
        for (int i = 0; i < yRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * yRaySpacing * Vector2.right, rayDirection, distanceToCast, LayerMask.GetMask("Solid"));

            //If there is a collision we will update velocity.y accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {

                velocity.y = (collision.distance - 0.01f) * direction;
                distanceToCast = collision.distance;
                
                collisionInfo.above = direction == 1;
                collisionInfo.below = direction == -1;
            }
        }

    }


    //Calculating Spacing and Locations for Rays to Be Cast
    //====================================================================================================

    private void updateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(0.01f * -2);

        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    }

    private void updateRaySpacing()
    {

        Bounds bounds = boxCollider.bounds;
        bounds.Expand(0.01f * -2);
        //We need at least 2 rays per side
        xRayCount = Mathf.Clamp(xRayCount, 2, int.MaxValue);
        yRayCount = Mathf.Clamp(yRayCount, 2, int.MaxValue);

        xRaySpacing = bounds.size.x / (xRayCount - 1);
        yRaySpacing = bounds.size.y / (yRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }
}
