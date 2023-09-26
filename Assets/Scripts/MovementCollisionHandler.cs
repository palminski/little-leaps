using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class MovementCollisionHandler : MonoBehaviour
{
    
    private BoxCollider2D boxCollider;

    [SerializeField]
    private float moveSpeed = 1;

    private RaycastOrigins raycastOrigins;
    
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        raycastOrigins.topRight = new Vector2 (boxCollider.bounds.max.x, boxCollider.bounds.max.y);
        raycastOrigins.topLeft = new Vector2 (boxCollider.bounds.min.x, boxCollider.bounds.max.y);
        
        // CheckForCollisionRight();
    }
    void FixedUpdate() {
        Move(moveSpeed);
    }

    void CheckForCollisionRight() {
        RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.topRight, Vector2.right, 1, LayerMask.GetMask("Solid"));
        if (collision.collider) Debug.Log("Hit " + collision.collider.name + " at a distance of " + collision.distance);
    }

    void Move(float speed) {
        float direction = Mathf.Sign(speed);
        Vector2 rayOrigin;
        

        if (direction > 0) {
            rayOrigin = raycastOrigins.topRight;
        }
        else {
            rayOrigin = raycastOrigins.topLeft;
        }
        Debug.DrawRay(rayOrigin, Vector2.right * moveSpeed, Color.magenta);
        RaycastHit2D collision = Physics2D.Raycast(rayOrigin, new Vector2 (direction,0), speed, LayerMask.GetMask("Solid"));
        if (collision.collider) {
            Debug.Log("Hitting " + collision.collider.name + " at a distance of " + collision.distance);
            transform.position += (new Vector3 (collision.distance * direction,0,0));
        } 
        else {
            transform.position += (new Vector3 (speed,0,0));
        }
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
