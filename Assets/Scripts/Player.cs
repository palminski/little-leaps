using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(MovementCollisionHandler))]
public class Player : MonoBehaviour
{

    private MovementCollisionHandler movementCollisionHandler;
    private Vector3 velocity;

    private float xInput;

    private float hSpeed;

    private bool jumpPressed;

    [Header("Horizontal Movement")]
    [SerializeField]
    private float moveSpeed = 0.4f;

    [SerializeField]
    private float acceleration = 0.1f;

    [SerializeField]
    private float groundFriction = 0.1f;

    [SerializeField]
    private float airFriction = 0.1f;

    [Header("Vertical Movement")]
    [SerializeField]
    private float jumpPower = 0.7f;

    [SerializeField]
    private float gravity = 0.05f;

    [Header("Wall Jump")]
    [SerializeField]
    private float distanceWallsDetectable = 0.5f;

    private void Awake()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
    }

    private void FixedUpdate()
    {





        //X Axis Speed
        //=========================================================
        //First we will start by checking players input and updating movespeed accordingly
        //if there is input then we increase the hSpeed by acceleration in that direction
        if (xInput != 0)
        {
            hSpeed += xInput * acceleration;
        }
        //Otherwise we will approach 0 by out friction ammount
        else
        {
            hSpeed = Mathf.MoveTowards(hSpeed, 0, groundFriction);
        }
        //limit the hSpeed by out movement speed in both directions
        hSpeed = Mathf.Clamp(hSpeed, -moveSpeed, moveSpeed);

        //Update the x component of velocity by hSpeed
        
            velocity.x = hSpeed;
        


        

        //Y Axis Speed
        //=========================================================
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) velocity.y = 0;
        velocity.y -= gravity;

        if (jumpPressed) velocity.y = jumpPower;
        jumpPressed = false;

        
        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * moveSpeed);

    }

    void OnJump()
    {
        // velocity.y = jumpPower;
        if (movementCollisionHandler.collisionInfo.below) jumpPressed = true;
        Debug.Log(movementCollisionHandler.OnWallAtDist(distanceWallsDetectable));
    }

    void OnMove(InputValue value)
    {
        float moveValue = value.Get<float>();
        xInput = moveValue;
    }
}
