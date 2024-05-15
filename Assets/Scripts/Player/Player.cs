using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;
using Unity.VisualScripting;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(MovementCollisionHandler))]
public class Player : MonoBehaviour
{

    private MovementCollisionHandler movementCollisionHandler;

    private PlayerInput playerInput;

    private InputAction jumpAction;
    private InputAction fastFallButton;
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    private Vector3 velocity;

    private float xInput;

    private float hSpeed;

    private bool jumpPressed;
    private bool jumpReleased;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 0.4f;

    [SerializeField] private float finalMoveSpeedScale = 0.4f;

    [SerializeField] private float acceleration = 0.1f;

    [SerializeField] private float fastFallModifier = 2;

    [SerializeField] private float groundFriction = 0.1f;

    [SerializeField] private float airFriction = 0.1f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpPower = 0.7f;

    [SerializeField] private float minJumpPower = 1;

    [SerializeField] private int coyoteTimeMax = 5;

    [SerializeField] private float jumpBuffer = 0.2f;

    [SerializeField] private float gravity = 0.05f;

    [SerializeField] private float terminalYVelocity = 1;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpPower = 0.5f;

    [SerializeField] private float wallJumpOffset = 0.5f;

    [SerializeField] private float wallJumpXPower = 0.5f;

    [SerializeField] private float distanceWallsDetectable = 0.5f;

    [SerializeField] private float wallClingGravityModifier = 0.4f;

    [Header("Damage")]
    [SerializeField] private float invincibilityTime = 3;
    [SerializeField] private float invincibilityBlinkInterval = 0.0001f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem ps;
    
    //

    private float extraForceX = 0;
    private float gravityModifier = 1;
    private int coyoteTime = 0;
    private Vector3 lastPosition;
    private float invincibilityCountdown;
    private float nextInvincibilityBlinkTime;

    public Vector3 startPosition;

    private float minJumpVelocity;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fastFallButton = playerInput.actions["FastFall"];
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        jumpAction = playerInput.actions["Jump"];
        jumpAction.started += context => OnJumpPress();
        jumpAction.canceled += context => OnJumpRelease();
    }

    private void Start()
    {


        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpPower);

        invincibilityCountdown = 0;

        startPosition = transform.position;

    }

    private void OnEnable()
    {
        jumpAction.Enable();
    }
    private void OnDisable()
    {
        jumpAction.Disable();
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
            hSpeed += Mathf.Sign(xInput) * acceleration;
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
        if (movementCollisionHandler.collisionInfo.left)
        {
            hSpeed = Mathf.Max(hSpeed,0);
            extraForceX = 0;
            
        }
        if (movementCollisionHandler.collisionInfo.right) {
            hSpeed = Mathf.Min(hSpeed,0);
            extraForceX = 0;
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
        if (xInput != 0 && velocity.y < 0 && movementCollisionHandler.OnWallAtDistInDirection(distanceWallsDetectable, (int)Mathf.Sign(xInput)))
        {
            gravityModifier = wallClingGravityModifier;
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

                    movementCollisionHandler.Move(new Vector3(wallJumpOffset * directionToJump, 0, 0));

                    velocity.y = wallJumpPower;
                    extraForceX = directionToJump * wallJumpXPower;
                    hSpeed = directionToJump * moveSpeed;
                }
            }
        }
        if (jumpReleased)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        velocity.y = Mathf.Clamp(velocity.y, -terminalYVelocity, terminalYVelocity);

        if (coyoteTime > 0) coyoteTime--;
        jumpPressed = false;
        jumpReleased = false;

        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * finalMoveSpeedScale);
        if (movementCollisionHandler.OnGround() && velocity.x != 0)
        {
            ps.Play();
        }
        if (movementCollisionHandler.InGround()) {
            StartCoroutine(WaitCheckAndDie());
        }
        

    }

    void Update()
    {
        //invincibility frames
        if (invincibilityCountdown > 0)
        {
            invincibilityCountdown -= Time.deltaTime;
            if (Time.time > nextInvincibilityBlinkTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                nextInvincibilityBlinkTime = Time.time + invincibilityBlinkInterval;
            }
        }
        else
        {
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

    void OnJumpPress()
    {
        jumpPressed = true;
    }

    void OnJumpRelease()
    {
        jumpReleased = true;
    }

    void OnMove(InputValue value)
    {
        float moveValue = value.Get<float>();
        xInput = moveValue;
    }

    void OnToggleRoom()
    {
        GameController.Instance.ToggleRoomState();
        

    }

    public void Damage()
    {
        transform.position = startPosition;
        GameController.Instance.ChangeHealth(-1);
        invincibilityCountdown = invincibilityTime;
        
    }

    public void Shove(int direction)
    {
        invincibilityCountdown = invincibilityTime;
        movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
        velocity.y = jumpPower * 0.5f;
        // extraForceX = -1 * wallJumpXPower;
        hSpeed = direction * moveSpeed;
    }

    //Methods to get player properties
    public bool IsInvincible()
    {
        return invincibilityCountdown > 0;
    }

    public bool IsFalling()
    {
        return velocity.y < -gravity * gravityModifier;
    }
    public bool IsInAir()
    {
        return !movementCollisionHandler.OnGround();
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    public void ResetCoyoteTime()
    {
        coyoteTime = coyoteTimeMax;
    }

    public void Bounce()
    {
        velocity.y = jumpPower;
    }

    private IEnumerator WaitCheckAndDie() 
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (movementCollisionHandler.InGround()) Damage();
    }
}
