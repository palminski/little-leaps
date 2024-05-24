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
    private float extraHSpeed = 0;
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

    // -------------------
    // Calculate Movement
    // -------------------
    private void FixedUpdate()
    {
        lastPosition = transform.position;
        bool isGrounded = movementCollisionHandler.OnGround();

        // -------------
        // X Axis Speed
        // -------------
        if (xInput != 0)
        {
            hSpeed += Mathf.Sign(xInput) * acceleration;
        }
        else
        {
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
            extraHSpeed = 0;
            
        }
        if (movementCollisionHandler.collisionInfo.right) {
            hSpeed = Mathf.Min(hSpeed,0);
            extraHSpeed = 0;
        }

        //we dont want extra force once we are on the ground. we also want to continuously move it back to 0
        if (isGrounded)
        {
            extraHSpeed = 0;
        }
        extraHSpeed = Mathf.MoveTowards(extraHSpeed, 0, groundFriction);

        //Update the x component of velocity by hSpeed and any additional extra force
        velocity.x = hSpeed + extraHSpeed;

        // -------------
        // Y Axis Speed
        // -------------
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
                    extraHSpeed = directionToJump * wallJumpXPower;
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

    // --------------------------
    // Update Called Every Frame
    // --------------------------
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

    
    // ------------------------------------
    // Methods that do something to player
    // ------------------------------------
    public void Damage()
    {
        if(IsInvincible()) return;
        // transform.position = startPosition;
        GameController.Instance.ChangeHealth(-1);
        invincibilityCountdown = invincibilityTime;
        
    }
    public void Shove(int direction)
    {
        invincibilityCountdown = invincibilityTime;
        movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
        velocity.y = jumpPower * 0.5f;
        // extraHSpeed = -1 * wallJumpXPower;
        hSpeed = direction * moveSpeed;
    }
    public void Bounce()
    {
        velocity.y = jumpPower;
    }
    public void ResetCoyoteTime()
    {
        coyoteTime = coyoteTimeMax;
    }
    private IEnumerator WaitCheckAndDie() 
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (movementCollisionHandler.InGround()) Damage();
    }

    // --------------------------------------
    // Methods used to get player Properties
    // --------------------------------------
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

    // --------------------------------
    // Methods used for handling input
    // --------------------------------
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
}
