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
        Debug.DrawRay(raycastOrigins.topRight, Vector2.right * moveSpeed, Color.magenta);
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
        
        RaycastHit2D collision = Physics2D.Raycast(raycastOrigins.topRight, Vector2.right, speed, LayerMask.GetMask("Solid"));
        if (collision.collider) {
            Debug.Log("Hitting " + collision.collider.name + " at a distance of " + collision.distance);
            transform.position += new Vector3 (collision.distance,0,0);
        } 
        else {
            transform.position += new Vector3 (speed,0,0);
        }
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
