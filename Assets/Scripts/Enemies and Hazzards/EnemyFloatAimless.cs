using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementCollisionHandler))]
public class EnemyFloatAimless : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.35f;

    [SerializeField]
    private Vector2 startDirection;
    
    private Vector2 direction = new(1,1);

    

    private MovementCollisionHandler movementCollisionHandler;
    
    
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        
    

        direction = startDirection.normalized;    
    }

    void FixedUpdate()
    {


        velocity = direction * speed;


        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
        
        if (movementCollisionHandler.collisionInfo.right || movementCollisionHandler.collisionInfo.left) direction.x = -direction.x;
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) direction.y = -direction.y;

    

    }

    // Update is called once per frame
    void Update()
    {

        if (direction.x != 0)
        {
            Vector3 newScale = new(Mathf.Sign(direction.x), 1, 1);
            transform.localScale = newScale;
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, startDirection.normalized, Color.cyan);
    }
}
