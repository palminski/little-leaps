using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformController : RaycastController
{

    private bool isPassable = false;
    private FallthroughSolid fallthroughSolid;
    public LayerMask passengerMask;
    public Vector3 move;

    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    [SerializeField]
    private bool shouldReverse = false;
    [SerializeField]
    private float waitTime = 0;
    [SerializeField][Range(0,2)]
    private float easeAmount = 0;
    [SerializeField]
    private float speed;
    int fromWaypointIndex;
    float percentBetweenWaypoints;

    private float nextMoveTime;



    private List<PassengerMovement> passengerMovements;
    private Dictionary<Transform, MovementCollisionHandler> passengerCollisionHandlers = new Dictionary<Transform, MovementCollisionHandler>();


    public override void Start()
    {
        base.Start();
        fallthroughSolid = GetComponent<FallthroughSolid>();
        if (GetComponent<FallthroughSolid>() != null) isPassable = true;

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();
        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);

        if (fallthroughSolid)
        {
            isPassable = fallthroughSolid.IsPassable();
        }
    }

    Vector3 CalculatePlatformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += speed / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (shouldReverse)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }
        return newPos - transform.position;
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }


    void MovePassengers(bool beforePlatMove)
    {
        foreach (PassengerMovement passenger in passengerMovements)
        {

            if (!passengerCollisionHandlers.ContainsKey(passenger.transform))
            {
                passengerCollisionHandlers.Add(passenger.transform, passenger.transform.GetComponent<MovementCollisionHandler>());
            }
            if (passenger.moveBeforePlatform == beforePlatMove)
            {
                passengerCollisionHandlers[passenger.transform].Move(passenger.velocity, passenger.onPlatform, false);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovements = new List<PassengerMovement>();

        float directionX = Math.Sign(velocity.x);
        float directionY = Math.Sign(velocity.y);

        //Moving Vertically===========================================================
        if (velocity.y != 0 && !isPassable)
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

                    // collision.transform.Translate(new Vector3(pushX, pushY));
                    passengerMovements.Add(new PassengerMovement(collision.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    movedPassengers.Add(collision.transform);

                }
            }
        }

        //Moving Vertically=============================================================
        if (velocity.x != 0 && !isPassable)
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
                    //recheck this
                    float pushY = 0;

                    // collision.transform.Translate(new Vector3(pushX, pushY));
                    passengerMovements.Add(new PassengerMovement(collision.transform, new Vector3(pushX, pushY), false, true));
                    movedPassengers.Add(collision.transform);

                }
            }
        }

        //Passenger on top of a platform not moving up====================================
        if (directionY == -1 && !isPassable || (velocity.y == 0 && velocity.x != 0) && !isPassable)
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

                    // collision.transform.Translate(new Vector3(pushX, pushY));
                    passengerMovements.Add(new PassengerMovement(collision.transform, new Vector3(pushX, pushY), true, false));
                    movedPassengers.Add(collision.transform);

                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool onPlatform;
        public bool moveBeforePlatform;

        //Constructor
        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _onPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            onPlatform = _onPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.magenta;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawSphere(globalWaypointPosition, size);
            }
        }
    }
}
