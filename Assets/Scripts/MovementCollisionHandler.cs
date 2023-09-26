using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementCollisionHandler : MonoBehaviour
{

    private BoxCollider2D boxCollider;

    [SerializeField]
    private Vector3 velocity;

    [SerializeField]
    private int xRayCount = 4;

    [SerializeField]
    private int yRayCount = 4;

    private float xRaySpacing;

    private float yRaySpacing;

    private RaycastOrigins raycastOrigins;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        updateRaySpacing();
    }

    // Update is called once per frame
    void Update()
    {
        // DebugFunctions to check ray lengths
        updateRaySpacing();
        for (int i = 0; i < xRayCount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomRight + Vector2.up * xRaySpacing * i, Vector2.right * velocity.x, Color.magenta);
        }
    }
    void FixedUpdate()
    {
        Move(velocity);
    }

    void Move(Vector3 velocity)
    {
        updateRaycastOrigins();

        CollisionsXAxis(ref velocity);

        transform.Translate(velocity);
    }

    void CollisionsXAxis(ref Vector3 velocity)
    {
        float xVelocity = velocity.x;
        float direction = Mathf.Sign(xVelocity);
        float distanceToCast = Mathf.Abs(xVelocity);

        Vector2 rayOrigin;
        Vector2 rayDirection = new Vector2 (direction,0);

        if (direction > 0)
        {
            rayOrigin = raycastOrigins.bottomRight;
        }
        else
        {
            rayOrigin = raycastOrigins.bottomLeft;
        }



        for (int i = 0; i < xRayCount; i++)
        {
            RaycastHit2D collision = Physics2D.Raycast(rayOrigin + Vector2.up * xRaySpacing * i, rayDirection, distanceToCast, LayerMask.GetMask("Solid"));

            if (collision.collider)
            {
                Debug.Log("Hitting " + collision.collider.name + " at a distance of " + collision.distance);
                velocity.x = collision.distance * direction;
                distanceToCast = collision.distance;
            }
        }


    }

    private void updateRaycastOrigins()
    {
        raycastOrigins.topRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.max.y);
        raycastOrigins.topLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.max.y);
        raycastOrigins.bottomRight = new Vector2(boxCollider.bounds.max.x, boxCollider.bounds.min.y);
        raycastOrigins.bottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
    }

    private void updateRaySpacing()
    {
        //We need at least 2 rays per side
        xRayCount = Mathf.Clamp(xRayCount, 2, int.MaxValue);
        yRayCount = Mathf.Clamp(yRayCount, 2, int.MaxValue);

        xRaySpacing = boxCollider.bounds.size.x / (xRayCount - 1);
        yRaySpacing = boxCollider.bounds.size.y / (yRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
