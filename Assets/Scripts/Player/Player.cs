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

    private PlayerInput playerInput;
    private InputAction fastFallButton;
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    private Vector3 velocity;

    private float xInput;

    private float hSpeed;

    private bool jumpPressed;

    [Header("Horizontal Movement")]
    [SerializeField]
    private float moveSpeed = 0.4f;

    [SerializeField]
    private float finalMoveSpeedScale = 0.4f;

    [SerializeField]
    private float acceleration = 0.1f;

    [SerializeField]
    private float fastFallModifier = 2;

    [SerializeField]
    private float groundFriction = 0.1f;

    [SerializeField]
    private float airFriction = 0.1f;

    [Header("Vertical Movement")]
    [SerializeField]
    private float jumpPower = 0.7f;

    [SerializeField]
    private int coyoteTimeMax = 5;

    [SerializeField]
    private float jumpBuffer = 0.2f;

    [SerializeField]
    private float gravity = 0.05f;

    [Header("Wall Jump")]
    [SerializeField]
    private float wallJumpPower = 0.5f;

    [SerializeField]
    private float wallJumpOffset = 0.5f;

    [SerializeField]
    private float wallJumpXPower = 0.5f;

    [SerializeField]
    private float distanceWallsDetectable = 0.5f;

    [Header("Damage")]
    [SerializeField]
    private float invincibilityTime = 3;
    [SerializeField]
    private float invincibilityBlinkInterval = 0.0001f;
    //

    private float extraForceX = 0;
    private float gravityModifier = 1;
    private int coyoteTime = 0;
    private Vector3 lastPosition;
    private float invincibilityCountdown;

    private float nextInvincibilityBlinkTime;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fastFallButton = playerInput.actions["FastFall"];
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        print(GameController.Instance.Score);
        GameController.Instance.AddToScore(1);
        print(GameController.Instance.AddToScore(1));
        print(GameController.Instance.Score);
        invincibilityCountdown = 0;
    }

    private void FixedUpdate()
    {
        //set last position to current position before moving
        lastPosition = transform.position;

        bool isGrounded = movementCollisionHandler.OnGround();
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
            if (isGrounded)
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
        if ((movementCollisionHandler.collisionInfo.left || movementCollisionHandler.collisionInfo.right))
        {
            hSpeed = 0;
        }

        //we dont want extra force once we are on the ground. we also want to continuously move it back to 0
        if (isGrounded)
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
        if (fastFallButton.IsPressed()) gravityModifier = fastFallModifier;
        if ((movementCollisionHandler.collisionInfo.left || movementCollisionHandler.collisionInfo.right) && velocity.x != 0 && velocity.y < 0)
        {
            gravityModifier = 0.4f;
        }

        velocity.y -= gravity * gravityModifier;

        //Jumping
        if (isGrounded) coyoteTime = coyoteTimeMax;

        if (jumpPressed)
        {
            if (coyoteTime > 0)
            {
                velocity.y = jumpPower;
            }
            else if (movementCollisionHandler.OnGroundAtDist(jumpBuffer))
            {
                velocity.y = jumpPower;
            }
            else
            {
                int directionToJump = 0;
                if (movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump))
                {
                    
                    movementCollisionHandler.Move(new Vector3(wallJumpOffset * directionToJump,0,0));
                    
                    velocity.y = wallJumpPower;
                    extraForceX = directionToJump * wallJumpXPower;
                    hSpeed = directionToJump * moveSpeed;
                }
            }
        }
        if (coyoteTime > 0) coyoteTime--;
        jumpPressed = false;

        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * finalMoveSpeedScale);
       

    }

    void Update()
    {
        //invincibility frames
        if (invincibilityCountdown > 0) {
            invincibilityCountdown -= Time.deltaTime;
            if (Time.time > nextInvincibilityBlinkTime) {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                nextInvincibilityBlinkTime = Time.time + invincibilityBlinkInterval;
            }
        } 
        else {
            spriteRenderer.enabled = true;
        }

        //Direction
        int inputDirection = System.Math.Sign(xInput);
        animator.SetInteger("input-direction", inputDirection);

        int hDirection = System.Math.Sign(hSpeed);

        if (hDirection != 0)
        {
            Vector3 newScale = new(hDirection, 1, 1);
            transform.localScale = newScale;
        }
        
        //falling animations 
        if (velocity.y > 0)
        {
            animator.SetBool("is-jumping", true);
            animator.SetBool("is-falling", false);

        }
        else if (velocity.y < -gravity * gravityModifier)
        {
            animator.SetBool("is-jumping", false);
            animator.SetBool("is-falling", true);

        }
        else
        {
            animator.SetBool("is-jumping", false);
            animator.SetBool("is-falling", false);
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

    void OnToggleRoom()
    {
        GameController.Instance.ToggleRoomState();
        print($"new room state = [{GameController.Instance.RoomState}]");

    }

    public void Damage() {
        
    }

    public void Shove(int direction) {
                    invincibilityCountdown = invincibilityTime;
                    movementCollisionHandler.Move(new Vector3(0,0.5f,0));
                    velocity.y = wallJumpPower/1.5f;
                    extraForceX = -1 * wallJumpXPower;
                    hSpeed = direction * moveSpeed;
    }

    //Methods to get player properties
    public bool IsInvincible() {
        return invincibilityCountdown > 0;
    }

    public Vector3 GetLastPosition() {
        return lastPosition;
    }

    public void ResetCoyoteTime() {
        coyoteTime = coyoteTimeMax;
    }

    public void Bounce() {
        velocity.y = jumpPower;
    }

}
