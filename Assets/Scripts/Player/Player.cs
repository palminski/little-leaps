using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public Vector3 startPosition;
    private Vector3 lastPosition;
    private float invincibilityCountdown;
    private float nextInvincibilityBlinkTime;

    [Header("Horizontal Movement")]
    [SerializeField] public float moveSpeed = 0.4f;
    [SerializeField] private float finalMoveSpeedScale = 0.4f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float fastFallModifier = 2;
    [SerializeField] private float groundFriction = 0.1f;
    [SerializeField] private float airFriction = 0.1f;
    private float xInput;
    private float hSpeed;
    private float hExtraSpeed = 0;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpPower = 0.7f;
    [SerializeField] private float minJumpPower = 1;
    [SerializeField] private int coyoteTimeMax = 5;
    [SerializeField] private float jumpBuffer = 0.2f;
    [SerializeField] private float gravity = 0.05f;
    [SerializeField] private float terminalYVelocity = 1;
    private bool jumpPressed;
    private bool jumpReleased;
    private float gravityModifier = 1;
    private int coyoteTime = 0;
    private float minJumpVelocity;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpPower = 0.5f;
    [SerializeField] private float wallJumpOffset = 0.5f;
    [SerializeField] private float wallJumpXPower = 0.5f;
    [SerializeField] private float distanceWallsDetectable = 0.5f;
    [SerializeField] private float distanceClingStarts = 0.2f;
    [SerializeField] private float wallClingGravityModifier = 0.4f;
    [SerializeField] private float wallJumpForgiveness = 0.02f;
    [SerializeField] private float clingTimeMax = 5;
    [SerializeField] private int maxWallJumpTime = 5;
    private float clingTime = 5;
    private int wallJumpTime = 0;

    private int directionToJump;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.4f;
    [SerializeField] private float fastFallhDuration = 0.2f;
    [SerializeField] private float xDashPower = 0.5f;
    [SerializeField] private float verticalDashPower = 0.2f;
    [SerializeField] private float downDashPower = 1f;
    [SerializeField] private float doubleJumpVelocityScaleX = 1;
    private bool canDash = true;
    private bool canDoubleJump = true;
    private bool isDashing = false;

    [Header("Damage")]
    [SerializeField] private float invincibilityTime = 3;
    [SerializeField] private float invincibilityBlinkInterval = 0.0001f;
    [SerializeField] private float wallOverlapThreshold = 0.005f;
    [SerializeField] private GameObject damageObject;
    [SerializeField] private GameObject respawnObject;

    [Header("Particles")]
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem playerAfterImage;




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
        playerAfterImage.Stop();
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
        if (coyoteTime <= 0 && velocity.x != 0 && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump))
        {
            wallJumpTime = maxWallJumpTime;
        }

        // -------------
        // X Axis Speed
        // -------------
        if (xInput != 0 && clingTime == 0)
        {
            hSpeed += Mathf.Sign(xInput) * acceleration;
        }
        else
        {
            if (isGrounded)
            {
                hSpeed = Mathf.MoveTowards(hSpeed, 0, groundFriction);
                GameController.Instance.EndPointCombo();
            }
            else
            {
                hSpeed = Mathf.MoveTowards(hSpeed, 0, airFriction);
            }
        }
        hExtraSpeed = Mathf.MoveTowards(hExtraSpeed, 0, airFriction);
        //limit the hSpeed by out movement speed in both directions
        hSpeed = Mathf.Clamp(hSpeed, -moveSpeed, moveSpeed);

        //if we are hitting a wall we set the hspeed to 0 so we can accelerate away from it quickly
        if (movementCollisionHandler.collisionInfo.left)
        {
            hSpeed = Mathf.Max(hSpeed, 0);
            hExtraSpeed = 0;

        }
        if (movementCollisionHandler.collisionInfo.right)
        {
            hSpeed = Mathf.Min(hSpeed, 0);
            hExtraSpeed = 0;
        }

        //Update the x component of velocity by hSpeed and any additional extra force
        velocity.x = (isDashing && velocity.y < 0) ? hExtraSpeed : hSpeed + hExtraSpeed;
        // velocity.x = hSpeed + hExtraSpeed;

        // -------------
        // Y Axis Speed
        // -------------
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below)
        {
            // velocity.y = 0;
            StartCoroutine(WaitAndSetVelocity());
        }

        gravityModifier = 1;
        if (fastFallButton.IsPressed()) gravityModifier = fastFallModifier;
        if (xInput != 0 && velocity.y < 0 && movementCollisionHandler.OnWallAtDistInDirection(distanceClingStarts, (int)Mathf.Sign(xInput)))
        {
            clingTime = clingTimeMax;
        }
        if (clingTime > 0 && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable) && !isDashing)
        {
            gravityModifier = wallClingGravityModifier;
        }

        if (!isDashing || velocity.y > 0) velocity.y -= gravity * gravityModifier;

        //
        // -- Jumping --
        //
        if (isGrounded)
        {
            coyoteTime = coyoteTimeMax;
            
            if (!isDashing) RefreshDashMoves();
        }
        //Check if player needs to be pushed out of a wall
        if (movementCollisionHandler.InGround())
        {
            if (movementCollisionHandler.CheckAndCorrectOverlap(wallOverlapThreshold))
            {
                Damage();
                // StartCoroutine(WaitCheckAndDamage());
            }
            else
            {
                coyoteTime = 0;
            }
        }
        if (jumpPressed)
        {
            if (coyoteTime > 0 || movementCollisionHandler.OnGroundAtDist(jumpBuffer))
            {
                if (coyoteTime <= 0) movementCollisionHandler.Move(new Vector3(0, -1, 0));
                clingTime = 0;
                velocity.y = jumpPower;
                GameController.Instance.EndPointCombo();
                StopDash();
                RefreshDashMoves();
            }
            else
            {
                if ((wallJumpTime > 0 || movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump)) && ((xInput == 0) || (xInput != 0 && Mathf.Sign(xInput) == Mathf.Sign(directionToJump))))
                {
                    clingTime = 0;
                    movementCollisionHandler.Move(new Vector3(wallJumpOffset * directionToJump, 0, 0));
                    velocity.y = wallJumpPower;
                    hExtraSpeed = directionToJump * wallJumpXPower;
                    hSpeed = directionToJump * moveSpeed;
                }
                else if (wallJumpTime > 0 || movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump))
                {
                    StartCoroutine(WaitAndTryWallJump(wallJumpForgiveness, directionToJump));
                }
                else if (canDoubleJump)
                {
                    Dash(90);
                }
            }
        }
        if (jumpReleased)
        {
            if (velocity.y > minJumpVelocity && !isDashing)
            {
                velocity.y = minJumpVelocity;
            }
        }

        float lowerLimit = isDashing ? downDashPower : terminalYVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -lowerLimit, terminalYVelocity * 5);

        if (coyoteTime > 0) coyoteTime--;
        if (clingTime > 0) clingTime--;
        if (wallJumpTime > 0) wallJumpTime--;
        if (wallJumpTime <= 0)
        {
            directionToJump = 0;
        }
        jumpPressed = false;
        jumpReleased = false;

        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * finalMoveSpeedScale);
        if (movementCollisionHandler.OnGround() && velocity.x != 0)
        {
            ps.Play();
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
    public void Damage(int damageDelt = 1, int directionToToss = 0)
    {
        if (IsInvincible()) return;
        // transform.position = startPosition;
        GameController.Instance.ChangeHealth(-damageDelt, false);
        // invincibilityCountdown = invincibilityTime;

    }

    public void HideAndStartRespawn()
    {

        GameLight light = GetComponentInChildren<GameLight>();
        GameObject newLight = Instantiate(light.gameObject, light.gameObject.transform);
        light = newLight.GetComponentInChildren<GameLight>();
        if (light)
        {
            light.transform.SetParent(null);
            light.Fade();
        }

        GameController.Instance.StartCoroutine(GameController.Instance.WaitAndReactivatePlayer(this, 1f));
        gameObject.SetActive(false);

        if (damageObject)
        {
            Instantiate(damageObject, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        }
    }

    public void Respawn()
    {
        invincibilityCountdown = invincibilityTime;
        xInput = 0;
        StopDash();
        velocity.x = 0;
        hSpeed = 0;
        velocity.y = 0;
        hExtraSpeed = 0;
        Vector3 newScale = new(transform.position.x < startPosition.x ? -1 : 1, 1, 1);
        transform.position = startPosition;
        transform.localScale = newScale;
        if (respawnObject)
        {
            Instantiate(respawnObject, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        }
        gameObject.SetActive(true);
        StopDash();
    }

    public void RemovePlayer()
    {
        GameLight light = GetComponentInChildren<GameLight>();
        if (light)
        {
            light.transform.SetParent(null);
            light.Fade();
            Destroy(gameObject);
        }
    }

    public void SetInputEnabled(bool shouldEnable)
    {
        playerInput.enabled = shouldEnable;
    }
    public void SetPlayerSpawnPointToTransform(Transform transform)
    {
        startPosition = transform.position;
    }
    public void Shove(int direction)
    {
        invincibilityCountdown = invincibilityTime;
        movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
        velocity.y = jumpPower * 0.5f;
        StopDash();
        hSpeed = direction * moveSpeed;
    }
    public void Bounce(float bounceMultiplier = 1f)
    {
        velocity.y = jumpPower * bounceMultiplier;
        if (gameObject.activeSelf) {
            StartCoroutine(StopDashingNextFrame());
        }
        
    }
    public void ResetCoyoteTime()
    {
        coyoteTime = coyoteTimeMax;
    }
    public void StopDash()
    {
        isDashing = false;
        playerAfterImage.Stop();
    }

    private void RefreshDashMoves()
    {
        canDash = true;
        canDoubleJump = true;
    }
    private IEnumerator WaitCheckAndDamage()
    {
        yield return new WaitForFixedUpdate();

        if (movementCollisionHandler.InGround()) Damage();
    }
    private IEnumerator HandleDashState(float durationOfDash)
    {
        isDashing = true;

        playerAfterImage.Play();
        yield return new WaitForSeconds(durationOfDash);
        isDashing = false;
        playerAfterImage.Stop();

        if (hExtraSpeed != 0) hSpeed = Mathf.Sign(hExtraSpeed) * moveSpeed;
        hExtraSpeed = 0;
        // if (xInput != 0) hSpeed = Mathf.Sign(xInput) * moveSpeed;
    }

    private IEnumerator WaitAndTryWallJump(float timeToWait, float direction)
    {
        float startingY = transform.position.y;
        float elapsedTime = 0f;
        while (elapsedTime < timeToWait)
        {
            elapsedTime += Time.deltaTime;
            if ((xInput != 0 && Mathf.Sign(xInput) == direction) || wallJumpTime > 0 && (Mathf.Sign(xInput) == direction || xInput == 0))
            {
                clingTime = 0;
                movementCollisionHandler.Move(new Vector3(wallJumpOffset * direction, 0, 0));
                velocity.y = wallJumpPower;
                hExtraSpeed = direction * wallJumpXPower;
                hSpeed = direction * moveSpeed;
                yield break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(timeToWait);
        float yAdjustment = startingY - transform.position.y;


        if (canDoubleJump)
        {
            movementCollisionHandler.Move(new Vector3(0, yAdjustment, 0));
            Dash(90);
        }
    }



    private IEnumerator StopDashingNextFrame()
    {
        yield return new WaitForFixedUpdate();
        StopDash();
        RefreshDashMoves();
    }

    private void Dash(float angle)
    {
        GameController.Instance.InvokePlayerDashed();
        // Fast Fall
        if (Mathf.Approximately(angle, -90f) && IsInAir())
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(fastFallhDuration));
            velocity.x = 0;
            hExtraSpeed = 0;
            hSpeed = 0;
            velocity.y = -downDashPower;
            return;
        }
        // Double Jump
        if (canDoubleJump && Mathf.Approximately(angle, 90f))
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(dashDuration));
            canDoubleJump = false;
            movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
            velocity.x = Mathf.Abs(velocity.x) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
            hSpeed = Mathf.Abs(hSpeed) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
            hExtraSpeed = 0;
            velocity.y = verticalDashPower;
            return;
        }
        // Dash Sideways
        if (canDash && Mathf.Approximately(angle, 0f) && !movementCollisionHandler.OnWallAtDistInDirection(0.001f, 1))
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(dashDuration));
            canDash = false;
            velocity.y = 0;
            hExtraSpeed = 1 * xDashPower;
            hSpeed = 1 * moveSpeed;
            return;
        }
        if (canDash && (Mathf.Approximately(angle, 180f) || Mathf.Approximately(angle, -180f)) && !movementCollisionHandler.OnWallAtDistInDirection(0.001f, -1))
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(dashDuration));
            canDash = false;
            velocity.y = 0;
            hExtraSpeed = -1 * xDashPower;
            hSpeed = -1 * moveSpeed;
            return;
        }
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
    public bool IsDashing()
    {
        return isDashing;
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
    void OnAttack()
    {
        Vector2 leftJoystickPosition = playerInput.actions["LeftJoystickTilt"].ReadValue<Vector2>();

        float angle = 0;
        if (leftJoystickPosition != Vector2.zero)
        {
            angle = Mathf.Atan2(leftJoystickPosition.y, leftJoystickPosition.x) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 90) * 90;
        }
        else if (transform.localScale.x == -1)
        {
            angle = 180;
        }
        Dash(angle);

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

    IEnumerator WaitAndSetVelocity()
    {
        yield return new WaitForFixedUpdate();
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below)
        {
            velocity.y = 0;
        }
    }
}
