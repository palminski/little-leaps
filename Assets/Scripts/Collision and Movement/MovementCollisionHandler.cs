using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementCollisionHandler : RaycastController
{
    //Define
    //====================================================================================================================




    public CollisionInfo collisionInfo;


    [SerializeField] private float collisionBufferX = 0;
    [SerializeField] private float collisionBufferY = 0;
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
        float distanceToCast = Mathf.Abs(xVelocity) + collisionBufferX;

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

                velocity.x = (collision.distance - skinWidth - collisionBufferX) * direction;
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
        float distanceToCast = Mathf.Abs(yVelocity) + collisionBufferY;

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

                velocity.y = (collision.distance - skinWidth - collisionBufferY) * direction;
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

    public bool OnGroundAtDist(float distance)
    {
        updateRaycastOrigins();
        for (int i = 0; i < yRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.bottomLeft + i * verticalRaySpacing * Vector2.right, Vector2.down, distance, collidableLayers);
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }

    public bool AreHazardsAfterMove(Vector3 adjustment)
    {
        Vector3 position = boxCollider.bounds.center;
        Vector2 size = boxCollider.bounds.size;
        float rotation = transform.eulerAngles.z;

        //check if we can get to the player from our current position
        RaycastHit2D hitBoxCast = Physics2D.BoxCast(
                    position,
                    size,
                    0f,
                    adjustment,
                    adjustment.magnitude,
                    collidableLayers
                );

        float distance = hitBoxCast.distance;


        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(position + adjustment.normalized * distance, size, rotation);

        foreach (Collider2D hit in hitObjects)
        {
            if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
            {
                return hazard.IsCurrentlyDangerous();
            }
        }
        return false;
    }

    public bool OnSpikeWallAtDistInDirection(float distance, int direction)
    {
        Vector2 rayOrigins = direction > 0 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        //Detects if there is a wall to the left or the right at the specified distance away or closer
        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D[] collisions = Physics2D.RaycastAll(rayOrigins + i * horizontalRaySpacing * Vector2.up, Vector2.right * direction, distance * 2);
            bool isWall = false;
            bool isSpike = false;
            foreach (RaycastHit2D collision in collisions)
            {
                if (collision.collider.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
                {
                    isSpike = true;
                }
                if (((1 << collision.collider.gameObject.layer) & collidableLayers) != 0)
                {
                    isWall = true;
                }
            }
            // if we find a patch of wall that is not covered by spikes we can wall jump
            if (isSpike == false && isWall == true)
            {
                return false;
            }
        }

        return true;
    }

    public bool OnGround()
    {
        Vector2 rayOrigin = raycastOrigins.bottomLeft;
        for (int i = 0; i < yRayCount; i++)
        {

            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.down, skinWidth + 0.001f, collidableLayers);

            //If there is a collision we will update velocity.y accordingly and decrease distance to cast as well so we dont cast subsequent rays too far and move into a block
            if (collision.collider)
            {
                return true;
            }
        }
        return false;
    }

    public bool InGround()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        boxSize -= Vector2.one * 0.045f;

        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter, boxSize, 0, collidableLayers);

        if (overlapCollider != null && overlapCollider.Distance(boxCollider).distance < 0.0201f)
        {
            return true;
        }

        return false;
    }

    public bool CheckAndCorrectOverlap(float threshold)
    {
        Vector2 boxCenter = boxCollider.bounds.center;
        Vector2 boxSize = boxCollider.bounds.size;
        Collider2D overlapCollider = Physics2D.OverlapBox(boxCenter, boxSize, 0, collidableLayers);
        if (overlapCollider != null)
        {

            var colliderDistance = boxCollider.Distance(overlapCollider);
            if (colliderDistance.isOverlapped && Mathf.Abs(colliderDistance.distance) <= Mathf.Abs(threshold))
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

    public void CorrectSmallHorizontalEdgeCollisions(Vector3 velocity, float offset = 0.3f)
    {
        // print("calculating dash offset");
        //Define some useful variables
        float xVelocity = velocity.x;
        if (xVelocity == 0) return;
        float direction = Mathf.Sign(xVelocity);
        float distanceToCast = Mathf.Abs(xVelocity) + collisionBufferX;

        Vector2 rayOriginBottom;
        Vector2 rayOriginBottomOffset;
        Vector2 rayOriginTop;
        Vector2 rayOriginTopOffset;
        Vector2 rayDirection = new Vector2(direction, 0);

        //Determine if moving left or right and decide where to start ray iteration
        if (direction > 0)
        {
            rayOriginBottom = raycastOrigins.bottomRight;
            rayOriginBottomOffset = raycastOrigins.bottomRight + new Vector2(0, offset);
            rayOriginTop = raycastOrigins.topRight;
            rayOriginTopOffset = raycastOrigins.topRight - new Vector2(0, offset);

        }
        else
        {
            rayOriginBottom = raycastOrigins.bottomLeft;
            rayOriginBottomOffset = raycastOrigins.bottomLeft + new Vector2(0, offset);
            rayOriginTop = raycastOrigins.topLeft;
            rayOriginTopOffset = raycastOrigins.topLeft - new Vector2(0, offset);
        }

        RaycastHit2D collisionBottom = Physics2D.Raycast(rayOriginBottom, rayDirection, distanceToCast, collidableLayers);
        RaycastHit2D collisionBottomOffset = Physics2D.Raycast(rayOriginBottomOffset, rayDirection, distanceToCast, collidableLayers);
        RaycastHit2D collisionTop = Physics2D.Raycast(rayOriginTop, rayDirection, distanceToCast, collidableLayers);
        RaycastHit2D collisionTopOffset = Physics2D.Raycast(rayOriginTopOffset, rayDirection, distanceToCast, collidableLayers);

        if (collisionBottom && !collisionBottomOffset)
        {
            float distToAdjustUp = offset;
            Vector2 originForBottomCalculation = new Vector2(collisionBottom.point.x, rayOriginBottomOffset.y);
            RaycastHit2D collisionFromOffset = Physics2D.Raycast(originForBottomCalculation, Vector2.down, offset, collidableLayers);
            if (collisionFromOffset)
            {
                distToAdjustUp = offset - Mathf.Abs(collisionFromOffset.distance) + 0.01f;
                //If there are hazards att aht position move up a little more
                Collider2D[] hitObjectsBottom = Physics2D.OverlapPointAll(collisionFromOffset.point);
                foreach (Collider2D hit in hitObjectsBottom)
                {
                    if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
                    {
                        if(hazard.IsCurrentlyDangerous()) distToAdjustUp += 0.1f;
                    }
                }

            }
            Move(new Vector2(0, distToAdjustUp));
        }
        if (collisionTop && !collisionTopOffset)
        {
            float distToAdjustDown = offset;
            Vector2 originForTopCalculation = new Vector2(collisionTop.point.x, rayOriginTopOffset.y);
            RaycastHit2D collisionFromTopOffset = Physics2D.Raycast(originForTopCalculation, Vector2.up, offset, collidableLayers);
            if (collisionFromTopOffset)
            {
                distToAdjustDown = -offset + Mathf.Abs(collisionFromTopOffset.distance) - 0.01f;

                //If there are hazards att aht position down up a little more
                Collider2D[] hitObjectsBottom = Physics2D.OverlapPointAll(collisionFromTopOffset.point);
                foreach (Collider2D hit in hitObjectsBottom)
                {
                    if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
                    {
                        if(hazard.IsCurrentlyDangerous()) distToAdjustDown -= 0.1f;
                    }
                }
            }
            Move(new Vector2(0, distToAdjustDown));
        }
    }

    public void CorrectSmallAboveEdgeCollisions(Vector3 velocity, float offset = 0.1f)
    {
        //Define some useful variables
        float yVelocity = velocity.y;
        if (yVelocity == 0) return;
        float direction = Mathf.Sign(yVelocity);
        float distanceToCast = Mathf.Abs(yVelocity) + 0.1f;

        Vector2 rayOriginLeft;
        Vector2 rayOriginLeftOffset;
        Vector2 rayOriginRight;
        Vector2 rayOriginRightOffset;
        Vector2 rayDirection = new Vector2(direction, 0);

        rayOriginLeft = raycastOrigins.topLeft;
        rayOriginLeftOffset = raycastOrigins.topLeft + new Vector2(offset, 0);
        rayOriginRight = raycastOrigins.topRight;
        rayOriginRightOffset = raycastOrigins.topRight - new Vector2(offset, 0);


        RaycastHit2D collisionLeft = Physics2D.Raycast(rayOriginLeft, Vector2.up, distanceToCast, collidableLayers);
        RaycastHit2D collisionLeftOffset = Physics2D.Raycast(rayOriginLeftOffset, Vector2.up, distanceToCast, collidableLayers);
        RaycastHit2D collisionRight = Physics2D.Raycast(rayOriginRight, Vector2.up, distanceToCast, collidableLayers);
        RaycastHit2D collisionRightOffset = Physics2D.Raycast(rayOriginRightOffset, Vector2.up, distanceToCast, collidableLayers);

        if (collisionLeft && !collisionLeftOffset)
        {
            float distToAdjustRight = offset;
            Vector2 originForLeftCalculation = new Vector2(rayOriginLeftOffset.x, collisionLeft.point.y);
            RaycastHit2D collisionFromOffset = Physics2D.Raycast(originForLeftCalculation, Vector2.left, offset, collidableLayers);
            if (collisionFromOffset)
            {
                distToAdjustRight = offset - Mathf.Abs(collisionFromOffset.distance) + 0.01f;
                //If there are hazards att aht position move up a little more
                Collider2D[] hitObjectsLeft = Physics2D.OverlapPointAll(collisionFromOffset.point + new Vector2(0,0.2f));
                foreach (Collider2D hit in hitObjectsLeft)
                {
                    
                    if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
                    {
                        
                        if(hazard.IsCurrentlyDangerous()) distToAdjustRight += 0.1f;
                    }
                }

            }
            Move(new Vector2(distToAdjustRight, 0));
        }

        if (collisionRight && !collisionRightOffset)
        {
            

            float distToAdjustLeft = offset;
            Vector2 originForRightCalculation = new Vector2(rayOriginRightOffset.x, collisionRight.point.y);
            RaycastHit2D collisionFromRightOffset = Physics2D.Raycast(originForRightCalculation, Vector2.right, offset, collidableLayers);
            if (collisionFromRightOffset)
            {
                distToAdjustLeft = offset - Mathf.Abs(collisionFromRightOffset.distance) + 0.01f;
                //If there are hazards att aht position move up a little more

                Collider2D[] hitObjectsRight = Physics2D.OverlapPointAll(collisionFromRightOffset.point  + new Vector2(0,0.2f));
                foreach (Collider2D hit in hitObjectsRight)
                {
                    if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
                    {
                        if(hazard.IsCurrentlyDangerous()) distToAdjustLeft += 0.1f;
                    }
                }

            }
            Move(new Vector2(-distToAdjustLeft, 0));
        }
        // if (collisionTop && !collisionTopOffset)
        // {
        //     float distToAdjustDown = offset;
        //     Vector2 originForTopCalculation = new Vector2(collisionTop.point.x, rayOriginTopOffset.y);
        //     RaycastHit2D collisionFromTopOffset = Physics2D.Raycast(originForTopCalculation, Vector2.up, offset, collidableLayers);
        //     if (collisionFromTopOffset)
        //     {
        //         distToAdjustDown = -offset + Mathf.Abs(collisionFromTopOffset.distance) - 0.01f;

        //         //If there are hazards att aht position down up a little more
        //         Collider2D[] hitObjectsBottom = Physics2D.OverlapPointAll(collisionFromTopOffset.point);
        //         foreach (Collider2D hit in hitObjectsBottom)
        //         {
        //             if (hit.TryGetComponent<HazardToPlayer>(out HazardToPlayer hazard))
        //             {
        //                 if(hazard.IsCurrentlyDangerous()) distToAdjustDown -= 0.1f;
        //             }
        //         }
        //     }
        //     Move(new Vector2(0, distToAdjustDown));
        // }
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
