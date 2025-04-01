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
    private float actionCooldown = 0;
    [SerializeField] private float MaxActionCooldown = 3;

    [Header("Horizontal Movement")]
    [SerializeField] public float moveSpeed = 0.4f;
    [SerializeField] private float finalMoveSpeedScale = 0.4f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float fastFallModifier = 2;
    [SerializeField] private float groundFriction = 0.1f;
    [SerializeField] private float airFriction = 0.1f;
    [SerializeField] private float horizontalDashCollisionForgivness = 0.3f;

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
    private float doubleJumpTurnAroundFrames;
    [SerializeField] private float maxDoubleJumpTurnAroundFrames = 5f;
    [SerializeField] private float verticalCollisionForgivness = 0.1f;
    private bool jumpPressed;
    private bool jumpReleased;
    private float gravityModifier = 1;

    private int coyoteTime = 0;
    private bool canWallJump = false;

    public int stompTimerMax = 5;
    private int stompTimer = 0;
    private float minJumpVelocity;
    private bool canCancelJump;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpPower = 0.5f;
    [SerializeField] private float wallJumpOffset = 0.5f;
    [SerializeField] private float wallJumpXPower = 0.5f;
    [SerializeField] private float distanceWallsDetectable = 0.5f;
    [SerializeField] private float distanceClingStarts = 0.2f;
    [SerializeField] private float wallClingGravityModifier = 0.4f;
    [SerializeField] private float wallJumpForgiveness = 0.02f;
    [SerializeField] private float clingTimeMax = 5;
    [SerializeField] private float maxClingSpeed = 0.1f;
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

    [SerializeField] private int maxDashChangeTime = 5;
    private int dashChangeTime = 0;


    [Header("Damage")]
    [SerializeField] private float invincibilityTime = 3;
    [SerializeField] private float invincibilityBlinkInterval = 0.0001f;
    [SerializeField] private float wallOverlapThreshold = 0.005f;
    [SerializeField] private GameObject damageObject;
    [SerializeField] private GameObject respawnObject;

    [Header("Particles")]
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem playerAfterImage;

    private bool hasClung;



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

        if (playerInput != null)
        {
            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.ReadValue().magnitude >= 0.1f)
                {
                    playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                    InputSystem.Update();

                }
            }
            if (InputController.Instance != null)
            {
                InputController.Instance.LoadBinding(playerInput);
            }
        }
    }

    // private void OnDestroy()
    // {
    //     print(playerInput.currentControlScheme);
    //     if (playerInput != null && InputController.Instance != null)
    //     {
    //         InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
    //     }
    // }

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
        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
        }
    }

    // -------------------
    // Calculate Movement
    // -------------------
    private void FixedUpdate()
    {
        if (stompTimer > 0)
        {
            stompTimer--;
        }
        lastPosition = transform.position;
        bool isGrounded = movementCollisionHandler.OnGround();
        if (coyoteTime <= 0 && movementCollisionHandler.OnWallAtDistInDirection(distanceWallsDetectable, (int)Mathf.Sign(transform.localScale.x)))
        {
            wallJumpTime = maxWallJumpTime;
        }

        // -------------
        // X Axis Speed
        // -------------
        if (xInput != 0 && clingTime == 0)
        {
            hSpeed += Mathf.Sign(xInput) * acceleration;
            if (doubleJumpTurnAroundFrames > 0)
            {
                velocity.x = Mathf.Abs(velocity.x) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
                hSpeed = Mathf.Abs(hSpeed) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
            }
            if (dashChangeTime > 0 && Mathf.Sign(xInput) != Mathf.Sign(transform.localScale.x))
            {
                canDash = true;
                dashChangeTime = 0;
                Dash(Mathf.Sign(xInput) > 0 ? 0 : 180, false);
            }
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

        if (doubleJumpTurnAroundFrames > 0)
        {
            doubleJumpTurnAroundFrames--;
        }

        if (actionCooldown > 0)
        {
            actionCooldown--;
        }


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
        //allow wall jumping if on wall in air
        if (!isGrounded && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable))
        {
            canWallJump = true;
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
        if (xInput != 0 && velocity.y < 0 && movementCollisionHandler.FullyOnWallAtDistInDirection(distanceClingStarts, (int)Mathf.Sign(xInput)) && (hasClung == false || clingTime > 0))
        {
            clingTime = clingTimeMax;
            hasClung = true;
        }
        if (clingTime > 0 && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable) && !isDashing)
        {
            gravityModifier = wallClingGravityModifier;
            velocity.y = Mathf.Max(-maxClingSpeed, velocity.y);
        }

        if (!isDashing || velocity.y > 0) velocity.y -= gravity * gravityModifier;

        //
        // -- Jumping --
        //
        if (isGrounded)
        {
            coyoteTime = coyoteTimeMax;
            canWallJump = false;
            if (!canDoubleJump)
            {
                canDoubleJump = true;
                GameController.Instance.InvokeUpdateJumpIcon();
            }

            hasClung = false;
            if (Mathf.Abs(velocity.x) <= moveSpeed) RefreshDashMoves();
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
        //-----------------------
        //Jump logic starts here
        //-----------------------
        if (jumpPressed && actionCooldown <= 0)
        {
            //on ground
            if (coyoteTime > 0 || movementCollisionHandler.OnGroundAtDist(jumpBuffer))
            {
                // change this to just a check for a hazard rather than actually moving the player
                // if (coyoteTime <= 0) movementCollisionHandler.Move(new Vector3(0, -1, 0));
                if (movementCollisionHandler.OnGround() || !movementCollisionHandler.AreHazardsAfterMove(new Vector3(0, -jumpBuffer, 0)))
                {
                    clingTime = 0;
                    if (gameObject.activeSelf) StartCoroutine(WaitAndJump());
                    GameController.Instance.EndPointCombo();
                    StopDash();
                    RefreshDashMoves();
                }
                else if (canDoubleJump)
                {
                    hasClung = false;
                    clingTime = 0;
                    StopDash();
                    DoubleJump();
                }
                else
                {
                    if (coyoteTime <= 0) movementCollisionHandler.Move(new Vector3(0, -1, 0));
                }

            }
            //in air
            else
            {
                //Start Wall Jump
                if (
                    canWallJump
                    && wallJumpTime > 0
                    && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump)
                    && xInput != 0 && Mathf.Sign(xInput) == Mathf.Sign(directionToJump)
                    // && !movementCollisionHandler.AreHazardsAfterMove(new(-distanceWallsDetectable, 0, 0))
                    // && !movementCollisionHandler.AreHazardsAfterMove(new(distanceWallsDetectable, 0, 0))
                    && !movementCollisionHandler.OnSpikeWallAtDistInDirection(distanceWallsDetectable, -directionToJump)

                    )
                {
                    //walljump
                    clingTime = 0;
                    hasClung = false;
                    movementCollisionHandler.Move(new Vector3(wallJumpOffset * -directionToJump, 0, 0));
                    velocity.y = wallJumpPower;
                    StartCoroutine(HandleAfterImage(dashDuration));
                    //Wait until after collisions resolve to make xaxis moves
                    if (gameObject.activeSelf) StartCoroutine(WaitAndWallJump(directionToJump));
                }
                else if (
                    canWallJump
                    && wallJumpTime > 0
                    && movementCollisionHandler.OnWallAtDist(distanceWallsDetectable, ref directionToJump)
                    // && !movementCollisionHandler.AreHazardsAfterMove(new(-distanceWallsDetectable, 0, 0))
                    // && !movementCollisionHandler.AreHazardsAfterMove(new(distanceWallsDetectable, 0, 0))
                    && !movementCollisionHandler.OnSpikeWallAtDistInDirection(distanceWallsDetectable, -directionToJump)
                    )
                {
                    //walljump
                    StartCoroutine(WaitAndTryWallJump(wallJumpForgiveness, directionToJump));
                }
                else if (canDoubleJump)
                {
                    hasClung = false;
                    clingTime = 0;
                    StopDash();
                    DoubleJump();
                }
            }
        }
        if (jumpReleased && canCancelJump)
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
        if (dashChangeTime > 0) dashChangeTime--;
        if (wallJumpTime <= 0)
        {
            directionToJump = 0;
        }
        jumpPressed = false;
        jumpReleased = false;

        //Make Minor Corrections To Avoid Edges
        if (!movementCollisionHandler.OnGround() && (Mathf.Abs(hExtraSpeed) > 0.1f))
        {
            movementCollisionHandler.CorrectSmallHorizontalEdgeCollisions(velocity * finalMoveSpeedScale, horizontalDashCollisionForgivness);
        }
        if (!movementCollisionHandler.OnGround() && Mathf.Abs(velocity.y) > 0f)
        {
            movementCollisionHandler.CorrectSmallAboveEdgeCollisions(velocity * finalMoveSpeedScale, verticalCollisionForgivness);
        }
        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity * finalMoveSpeedScale);
        if (movementCollisionHandler.OnGround() && velocity.x != 0)
        {
            ps.Play();
        }

    }
    private IEnumerator WaitAndJump()
    {
        if (actionCooldown <= 0)
        {
            yield return new WaitForFixedUpdate();
            velocity.y = jumpPower;
            if (AudioController.Instance != null) AudioController.Instance.PlayJump();
        }


    }

    private IEnumerator WaitAndWallJump(int directionToJump)
    {
        yield return new WaitForFixedUpdate();
        movementCollisionHandler.Move(new Vector3(wallJumpOffset * directionToJump, 0, 0));
        hExtraSpeed = directionToJump * wallJumpXPower;
        hSpeed = directionToJump * moveSpeed;
        if (AudioController.Instance != null) AudioController.Instance.PlayJump();

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
    public void Damage(int damageDelt = 1, int directionToToss = 0, bool ignoreImmune = false)
    {
        if (GameController.Instance.Health > 0 && !ignoreImmune && IsInvincible()) return;
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
        jumpPressed = false;
        Vector3 newScale = new(transform.position.x < startPosition.x ? -1 : 1, 1, 1);
        transform.position = startPosition;
        transform.localScale = newScale;
        // if (respawnObject)
        // {
        //     Instantiate(respawnObject, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        // }
        gameObject.SetActive(true);
        StopDash();
        if (playerInput != null)
        {
            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.ReadValue().magnitude >= 0.1f)
                {
                    playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                    InputSystem.Update();
                }
            }
            if (InputController.Instance != null)
            {
                InputController.Instance.LoadBinding(playerInput);
            }
        }
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
        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
        }
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
        stompTimer = stompTimerMax;
        // StartCoroutine(WaitAndJump(jumpPower * bounceMultiplier));
        canCancelJump = false;
        velocity.y = jumpPower * bounceMultiplier;
        actionCooldown = MaxActionCooldown;
        if (gameObject.activeSelf)
        {
            StartCoroutine(StopDashingNextFrame());
        }

    }
    public void Boost(float boostAmmount)
    {
        hSpeed = moveSpeed * Mathf.Sign(boostAmmount);
        hExtraSpeed = boostAmmount;
        StartCoroutine(WaitAndHandleAfterImage(dashDuration));
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

    public void RefreshDashMoves()
    {
        canDash = true;
        canDoubleJump = true;
        GameController.Instance.InvokeUpdateJumpIcon();
        GameController.Instance.InvokeUpdateDashIcon();
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
    }

    private IEnumerator HandleAfterImage(float durationOfDash)
    {
        playerAfterImage.Play();
        yield return new WaitForSeconds(durationOfDash);
        playerAfterImage.Stop();
    }

    private IEnumerator WaitAndHandleAfterImage(float durationOfDash)
    {
        yield return new WaitForFixedUpdate();
        playerAfterImage.Play();
        yield return new WaitForSeconds(durationOfDash);
        playerAfterImage.Stop();
    }

    private IEnumerator WaitAndTryWallJump(float timeToWait, float direction)
    {
        bool startedDoubleJump = false;
        if (canDoubleJump)
        {
            hasClung = false;
            clingTime = 0;
            StopDash();
            startedDoubleJump = true;
            DoubleJumpWithoutStoppingCoroutines();
        }




        float elapsedTime = 0f;
        while (elapsedTime < timeToWait)
        {
            elapsedTime += Time.deltaTime;
            if (xInput != 0 && Mathf.Sign(xInput) == direction)
            {
                clingTime = 0;
                hasClung = false;
                // float yAdjustmentPreDoubleJump = startingY - transform.position.y;
                // movementCollisionHandler.Move(new Vector3(wallJumpOffset * direction, yAdjustmentPreDoubleJump, 0));
                velocity.y = wallJumpPower;
                hExtraSpeed = direction * wallJumpXPower;
                hSpeed = direction * moveSpeed;

                if (startedDoubleJump)
                {
                    canDoubleJump = true;
                }
                yield break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(timeToWait);
        GameController.Instance.InvokeUpdateJumpIcon();

        // clingTime = 0;
        // hasClung = false;
        // float yAdjustment = startingY - transform.position.y;


        // if (canDoubleJump)
        // {
        //     movementCollisionHandler.Move(new Vector3(0, yAdjustment, 0));
        //     DoubleJump();
        // }
    }



    private IEnumerator StopDashingNextFrame()
    {
        yield return new WaitForFixedUpdate();
        StopDash();
        RefreshDashMoves();
    }

    private void Dash(float angle, bool allowDashChange = true)
    {
        if (allowDashChange) dashChangeTime = maxDashChangeTime;
        GameController.Instance.InvokePlayerDashed();
        // Dash Sideways
        if (canDash && Mathf.Approximately(angle, 0f) && !movementCollisionHandler.OnWallAtDistInDirection(0.05f, 1))
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(dashDuration));
            canDash = false;
            GameController.Instance.InvokeUpdateDashIcon();
            if (actionCooldown <= 0)
            {
                velocity.y = 0;
            }
            hExtraSpeed = 1 * xDashPower;
            hSpeed = 1 * moveSpeed;
            hasClung = false;
            clingTime = 0;
            if (AudioController.Instance != null) AudioController.Instance.PlayDash();

            return;
        }
        if (canDash && (Mathf.Approximately(angle, 180f) || Mathf.Approximately(angle, -180f)) && !movementCollisionHandler.OnWallAtDistInDirection(0.05f, -1))
        {
            StopAllCoroutines();
            StartCoroutine(HandleDashState(dashDuration));
            canDash = false;
            GameController.Instance.InvokeUpdateDashIcon();
            if (actionCooldown <= 0)
            {
                velocity.y = 0;
            }
            hExtraSpeed = -1 * xDashPower;
            hSpeed = -1 * moveSpeed;
            hasClung = false;
            clingTime = 0;
            if (AudioController.Instance != null) AudioController.Instance.PlayDash();

            return;
        }
    }

    private void NewDash()
    {

    }
    private void DoubleJump()
    {
        StopAllCoroutines();
        StartCoroutine(HandleAfterImage(dashDuration));
        canDoubleJump = false;
        GameController.Instance.InvokeUpdateJumpIcon();
        movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
        velocity.x = Mathf.Abs(velocity.x) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
        hSpeed = Mathf.Abs(hSpeed) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
        hExtraSpeed = 0;
        velocity.y = verticalDashPower;
        if (AudioController.Instance != null) AudioController.Instance.PlayAirJump();

        doubleJumpTurnAroundFrames = maxDoubleJumpTurnAroundFrames;
    }
    private void DoubleJumpWithoutStoppingCoroutines()
    {
        StartCoroutine(HandleAfterImage(dashDuration));
        canDoubleJump = false;
        movementCollisionHandler.Move(new Vector3(0, 0.01f, 0));
        velocity.x = Mathf.Abs(velocity.x) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
        hSpeed = Mathf.Abs(hSpeed) * doubleJumpVelocityScaleX * Mathf.Sign(xInput);
        hExtraSpeed = 0;
        velocity.y = verticalDashPower;
        if (AudioController.Instance != null) AudioController.Instance.PlayAirJump();

        doubleJumpTurnAroundFrames = maxDoubleJumpTurnAroundFrames;
    }
    private void Stomp()
    {
        if (stompTimer > 0) return;
        StopAllCoroutines();
        StartCoroutine(HandleDashState(fastFallhDuration));
        velocity.x = 0;
        hExtraSpeed = 0;
        hSpeed = 0;
        if (AudioController.Instance != null) AudioController.Instance.PlayDash();
        velocity.y = -downDashPower;
        return;
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
    public bool IsGrounded()
    {
        return movementCollisionHandler.OnGround();
    }
    public bool IsInAir()
    {
        return !movementCollisionHandler.OnGround();
    }
    public bool IsDashing()
    {
        return isDashing;
    }
    public bool IsMoving()
    {
        return velocity.x != 0;
    }
    public bool IsDashingSideways()
    {
        return hExtraSpeed != 0 && isDashing;
    }
    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }
    public bool PressedJump()
    {
        return jumpPressed;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public bool CanDoubleJump()
    {
        return canDoubleJump;
    }

    public bool CanDash()
    {
        return canDash;
    }

    // --------------------------------
    // Methods used for handling input
    // --------------------------------
    void OnJumpPress()
    {
        canCancelJump = true;
        jumpPressed = true;
    }
    void OnJumpRelease()
    {
        jumpReleased = true;
    }
    void OnAttack()
    {
        Vector2 leftJoystickPosition = playerInput.actions["LeftJoystickTilt"].ReadValue<Vector2>();
        float moveInputDirection = playerInput.actions["Move"].ReadValue<float>();

        if (fastFallButton.ReadValue<float>() < -0.85f)
        {
            Stomp();
            return;
        }

        float angle = 0;

        if (moveInputDirection == -1)
        {
            angle = 180;
        }
        else if (moveInputDirection == 1)
        {
            angle = 0;
        }
        else if (transform.localScale.x == -1)
        {
            angle = 180;
        }
        if (canDash) Dash(angle);

    }
    void OnMove(InputValue value)
    {
        float moveValue = value.Get<float>();
        xInput = moveValue;
    }

    void OnStomp()
    {
        if (!IsGrounded())
        {
            Stomp();
        }
    }
    void OnToggleRoom()
    {
        if (Time.timeScale == 0) return;
        GameController.Instance.ToggleRoomState();
    }
    void OnOpenMenu()
    {
        GameController.Instance.OpenPauseMenu();
    }

    IEnumerator WaitAndSetVelocity()
    {
        // if (velocity.y < -0.1f) if (AudioController.Instance != null) AudioController.Instance.PlayLandings();

        yield return new WaitForFixedUpdate();
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below)
        {
            velocity.y = 0;
        }
    }
}
