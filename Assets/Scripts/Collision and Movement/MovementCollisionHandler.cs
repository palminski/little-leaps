using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementCollisionHandler : RaycastController
{
    //Define
    //====================================================================================================================




    public CollisionInfo collisionInfo;

    //==debug==
    [Header("Debug Settings")]
    public bool shouldDrawRays = false;

    //====================================================================================================================

    public void Move(Vector3 velocity, bool standingOnPLatform = false, bool resetCollisionInfo = true)
    {
        
        //reset current collision info and find where we should be casting rays from
        if (resetCollisionInfo) collisionInfo.Reset();

        updateRaycastOrigins();

        //cast horizontal rays to determine how far we are allowed to move horizontally and make that our x velocity
        if (velocity.x != 0) CollisionsXAxis(ref velocity);

        //now we offset where we will cast our vertical rays by how much we plan to move on the x axis so that we dont go into a corner
        addToRaycastOriginsX(velocity.x);

        //cast vertical rays and adjust our y velocity accordingly
        if (velocity.y != 0) CollisionsYAxis(ref velocity);

        //update our actual position based on potentially updated velocity values
        if (!InGround()) transform.Translate(velocity);

        if (standingOnPLatform) collisionInfo.below = true;

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

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * horizontalRaySpacing * Vector2.up, rayDirection, distanceToCast, collidableLayers);
            if (shouldDrawRays) Debug.DrawRay(rayOrigin + i * horizontalRaySpacing * Vector2.up, rayDirection, Color.red, distanceToCast);
            //If there is a collision we will update velocity.x accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {

                velocity.x = (collision.distance - skinWidth) * direction;
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

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, rayDirection, distanceToCast, collidableLayers);
            if (shouldDrawRays) Debug.DrawRay(rayOrigin + i * verticalRaySpacing * Vector2.right, rayDirection, Color.red, distanceToCast);

            //If there is a collision we will update velocity.y accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {

                velocity.y = (collision.distance - skinWidth) * direction;
                distanceToCast = collision.distance;

                collisionInfo.above = direction == 1;
                collisionInfo.below = direction == -1;
            }
        }

    }
    //Public Methods
    //====================================================================================================
    public bool OnWallAtDist(float distance, ref int outDirection)
    {
        //Detects if there is a wall to the left or the right at the specified distance away or closer
        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.bottomRight + i * horizontalRaySpacing * Vector2.up, Vector2.right, distance, collidableLayers);
            if (collision.collider)
            {
                outDirection = -1;
                return true;
            }
            collision = Physics2D.Raycast(raycastOrigins.bottomLeft + i * horizontalRaySpacing * Vector2.up, Vector2.left, distance, collidableLayers);
            if (collision.collider)
            {
                outDirection = 1;
                return true;
            }
        }
        return false;
    }

    public bool OnWallAtDist(float distance)
    {
        int dummyDirection = 0;
        return OnWallAtDist(distance, ref dummyDirection);
    }

    public bool OnWallAtDistInDirection(float distance, int direction)
    {
        Vector2 rayOrigins = direction > 0 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        //Detects if there is a wall to the left or the right at the specified distance away or closer
        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(rayOrigins + i * horizontalRaySpacing * Vector2.up, Vector2.right * direction, distance, collidableLayers);
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }

    public bool FullyOnWallAtDistInDirection(float distance, int direction)
    {
        Vector2 rayOrigins = direction > 0 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        //Detects if there is a wall to the left or the right at the specified distance away or closer
        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(rayOrigins + i * horizontalRaySpacing * Vector2.up, Vector2.right * direction, distance, collidableLayers);
            if (!collision.collider)
            {
                return false;
            }
        }
        return true;
    }

    public bool OnGroundAtDist(float distance) {
        updateRaycastOrigins();
        for (int i = 0; i < yRayCount; i++) {
            RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.bottomLeft + i * verticalRaySpacing * Vector2.right, Vector2.down, distance, collidableLayers);
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }

    public bool OnGround() {
        Vector2 rayOrigin = raycastOrigins.bottomLeft;
        for (int i = 0; i < yRayCount; i++)
        {

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.down, skinWidth+0.001f, collidableLayers);

            //If there is a collision we will update velocity.y accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }

    public bool InGround() {
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;
        
        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);
        // if (overlapCollider != null) 
        // {
        //     print(overlapCollider.Distance(boxCollider).distance);
        // }
        // else {
        //     print("No Collision");
        // }
        if (overlapCollider != null && overlapCollider.Distance(boxCollider).distance < 0.0201f) {
            return true;
        } 

        return false;
    }

    public bool CheckAndCorrectOverlap(float threshold) {
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);
        if (overlapCollider != null)
        {

            var colliderDistance = boxCollider.Distance(overlapCollider);
            print(colliderDistance.distance + " - "+threshold);
            if (colliderDistance.isOverlapped &&  Mathf.Abs(colliderDistance.distance) <= Mathf.Abs(threshold))
            {
                transform.position += (Vector3)colliderDistance.normal * colliderDistance.distance;
                if (InGround())
                {
                    return true;
                }
                return false;
                
            }
            return true;
        }
        return false;
        
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Collider2D boxCollider = GetComponent<BoxCollider2D>();;
    //     Vector2 boxCenter = boxCollider.bounds.center;
    //     Vector2 boxSize = boxCollider.bounds.size;
    //     boxSize -= Vector2.one * skinWidth;
        
    //     // Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter,boxSize,0,collidableLayers);

    //     Gizmos.DrawWireCube(boxCenter, boxSize);
    // }



    //Structs
    //==============================================================================




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
