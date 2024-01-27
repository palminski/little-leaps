using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MovementCollisionHandler))]
public class EnemyFloatAimless : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.35f;

    [SerializeField]
    private bool startRight = true;

    [SerializeField]
    private bool startUp = true;


    

    private MovementCollisionHandler movementCollisionHandler;
    private Collider2D collider2d;
    private int directionX;
    private int directionY;
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        collider2d = GetComponent<Collider2D>();
        directionX = startRight ? 1 : -1;
        directionY = startUp ? 1 : -1;
    }

    void FixedUpdate()
    {


        velocity.x = directionX * speed;
        velocity.y = directionY * speed;


        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
        if (movementCollisionHandler.collisionInfo.right) directionX = -1;
        if (movementCollisionHandler.collisionInfo.left) directionX = 1;
        if (movementCollisionHandler.collisionInfo.above) directionY = -1;
        if (movementCollisionHandler.collisionInfo.below) directionY = 1;
    

    }

    // Update is called once per frame
    void Update()
    {

        if (directionX != 0)
        {
            Vector3 newScale = new(directionX, 1, 1);
            transform.localScale = newScale;
        }
    }


    private void TurnAround()
    {
        directionX = -directionX;
    }
}
