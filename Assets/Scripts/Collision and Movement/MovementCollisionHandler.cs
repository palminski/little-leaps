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
    // Start is called before the first frame update


    public void Move(Vector3 velocity, bool standingOnPLatform = false)
    {
        //reset current collision info and find where we should be casting rays from
        collisionInfo.Reset();

        updateRaycastOrigins();

        //cast horizontal rays to determine how far we are allowed to move horizontally and make that our x velocity
        if (velocity.x != 0) CollisionsXAxis(ref velocity);

        //now we offset where we will cast our vertical rays by how much we plan to move on the x axis so that we dont go into a corner
        addToRaycastOriginsX(velocity.x);

        //cast vertical rays and adjust our y velocity accordingly
        if (velocity.y != 0) CollisionsYAxis(ref velocity);

        //update our actual position based on potentially updated velocity values
        transform.Translate(velocity);

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

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * horizontalRaySpacing * Vector2.up, rayDirection, distanceToCast, LayerMask.GetMask("Solid"));
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

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, rayDirection, distanceToCast, LayerMask.GetMask("Solid"));
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
            RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.bottomRight + i * horizontalRaySpacing * Vector2.up, Vector2.right, distance, LayerMask.GetMask("Solid"));
            //If there is a collision we will update velocity.x accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                outDirection = -1;
                return true;
            }
            collision = Physics2D.Raycast(raycastOrigins.bottomLeft + i * horizontalRaySpacing * Vector2.up, Vector2.left, distance, LayerMask.GetMask("Solid"));
            //If there is a collision we will update velocity.x accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                outDirection = 1;
                return true;
            }
        }
        return false;
    }

    public bool OnGround() {
        Vector2 rayOrigin = raycastOrigins.bottomLeft;
        for (int i = 0; i < yRayCount; i++)
        {

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.down, skinWidth+0.001f, LayerMask.GetMask("Solid"));

            //If there is a collision we will update velocity.y accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }



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
