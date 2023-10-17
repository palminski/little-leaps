using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

    public LayerMask passengerMask;
    public Vector3 move;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        updateRaycastOrigins();

        Vector3 velocity = move;
        MovePassengers(velocity);
        transform.Translate(velocity);
    }

    void MovePassengers(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        float directionX = Math.Sign(velocity.x);
        float directionY = Math.Sign(velocity.y);

        //Moving Vertically===========================================================
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            Vector2 rayOrigin;
            if (directionY > 0)
            {
                rayOrigin = raycastOrigins.topLeft;
            }
            else
            {
                rayOrigin = raycastOrigins.bottomLeft;
            }

            for (int i = 0; i < yRayCount; i++)
            {
                RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.up * directionY, rayLength, passengerMask);
                if (collision.transform && !movedPassengers.Contains(collision.transform))
                {

                    float pushX = (directionY == 1) ? velocity.x : 0;
                    float pushY = velocity.y - (collision.distance - skinWidth) * directionY;

                    collision.transform.Translate(new Vector3(pushX, pushY));
                    movedPassengers.Add(collision.transform);

                }
            }
        }

        //Moving Vertically=============================================================
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin;
            if (directionX > 0)
            {
                rayOrigin = raycastOrigins.bottomRight;
            }
            else
            {
                rayOrigin = raycastOrigins.bottomLeft;
            }

            for (int i = 0; i < xRayCount; i++)
            {
                RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * horizontalRaySpacing * Vector2.up, Vector2.right * directionX, rayLength, passengerMask);
                if (collision.transform && !movedPassengers.Contains(collision.transform))
                {

                    float pushX = velocity.x - (collision.distance - skinWidth) * directionX;
                    float pushY = 0;

                    collision.transform.Translate(new Vector3(pushX, pushY));
                    movedPassengers.Add(collision.transform);

                }
            }
        }

        //Passenger on top of a platform not moving up====================================
        if (directionY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float rayLength = skinWidth * 2;
            Vector2 rayOrigin = raycastOrigins.topLeft;


            for (int i = 0; i < yRayCount; i++)
            {
                
                RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.up, rayLength, passengerMask);
                if (collision.transform && !movedPassengers.Contains(collision.transform))
                {

                    float pushX = velocity.x;
                    float pushY = velocity.y;

                    collision.transform.Translate(new Vector3(pushX, pushY));
                    movedPassengers.Add(collision.transform);

                }
            }
        }
    }
}
