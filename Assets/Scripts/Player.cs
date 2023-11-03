using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(MovementCollisionHandler))]
public class Player : MonoBehaviour
{

    private MovementCollisionHandler movementCollisionHandler;

    private Animator animator;

    
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
    private float wallJumpPower = 0.5f;

    [SerializeField]
    private float wallJumpXPower = 0.5f;

    [SerializeField]
    private float distanceWallsDetectable = 0.5f;


    //
    private float extraForceX = 0;
    private float gravityModifier = 1;
    private void Awake()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        animator = GetComponent<Animator>();
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
            //Determine weather to use air or ground friction
            if (movementCollisionHandler.collisionInfo.below)
            {
                hSpeed = Mathf.MoveTowards(hSpeed, 0, groundFriction);
            }
            else
            {
                hSpeed = Mathf.MoveTowards(hSpeed, 0, airFriction);
            }
        }
        //limit the hSpeed by out movement speed in both directions
        hSpeed = Mathf.Clamp(hSpeed, -moveSpeed, moveSpeed);

        //if we are hitting a wall we set the hspeed to 0 so we can accelerate away from it quickly
        if ((movementCollisionHandler.collisionInfo.left || movementCollisionHandler.collisionInfo.right) && movementCollisionHandler.collisionInfo.below)
        {
            hSpeed = 0;
        }

        //we dont want extra force once we are on the ground. we also want to continuously move it back to 0
        if (movementCollisionHandler.collisionInfo.below)
        {
            extraForceX = 0;
        }
        extraForceX = Mathf.MoveTowards(extraForceX, 0, groundFriction);

        //Update the x component of velocity by hSpeed and any additional extra force
        velocity.x = hSpeed + extraForceX;


        //Y Axis Speed
        //=========================================================
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) velocity.y = 0;

        gravityModifier = 1;
        if ((movementCollisionHandler.collisionInfo.left || movementCollisionHandler.collisionInfo.right) && velocity.x != 0 && velocity.y < 0)
        {
            gravityModifier = 0.4f;
        }

        velocity.y -= gravity * gravityModifier;


        if (jumpPressed)
        {

            if (movementCollisionHandler.collisionInfo.below)
            {
                velocity.y = jumpPower;
            }
            else
            {
                int directionToJump = 0;
                if (movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump)) velocity.y = wallJumpPower;

                extraForceX = directionToJump * wallJumpXPower;
                hSpeed = directionToJump * moveSpeed;
            }
        }
        jumpPressed = false;


        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * moveSpeed);

    }

    void Update()
    {
        int inputDirection = System.Math.Sign(xInput);
        animator.SetInteger("input-direction", inputDirection);

        int hDirection = System.Math.Sign(hSpeed);

        if (hDirection != 0) {
            Vector3 newScale = new(hDirection,1,1);
            transform.localScale = newScale;
        }

        //falling animations 
        if (velocity.y > 0) {
            animator.SetBool("is-jumping", true);
            animator.SetBool("is-falling",false);
            print("jump");
        }
        else if (velocity.y < -gravity){
            animator.SetBool("is-jumping",false);
            animator.SetBool("is-falling",true);
            print("fall");
        }
        else {
            animator.SetBool("is-jumping",false);
            animator.SetBool("is-falling",false);
            print("grounded");
        }
    }

    void OnJump()
    {
        // velocity.y = jumpPower;
        jumpPressed = true;

    }

    void OnMove(InputValue value)
    {
        float moveValue = value.Get<float>();
        
        xInput = moveValue;
    }

    void OnFastFall()
    {
        print("fast fall");
    }
}
