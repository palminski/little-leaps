using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : RaycastController
{

    public float boostAmmount;
    public LayerMask passengerMask;
    private Animator animator;

    private List<PassengerMovement> passengerMovements;
    private Dictionary<Transform, MovementCollisionHandler> passengerCollisionHandlers = new Dictionary<Transform, MovementCollisionHandler>();

    [SerializeField] private float speed = 0.2f;
    private Player player;
    private float startingDirection;

    private float boostDirection;

    public enum MovementBehavior
    {
        Default,
        ReverseOnChange
    }

    [SerializeField] private RoomColor activeOnRoomColor;
    public MovementBehavior behavior = MovementBehavior.Default;


    public override void Start()
    {
        base.Start();
        
        startingDirection = Mathf.Sign(speed);

        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

        animator.speed = Mathf.Abs(speed * 28f);
        boostDirection = Mathf.Sign(speed);
        transform.localScale = new(Mathf.Sign(speed),1,1);
        if (behavior != MovementBehavior.Default)
        {
            HandleRoomStateChange();
        }
    }

    private void OnEnable()
    {
        if (behavior != MovementBehavior.Default)
        {
            GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        }
        
        
    }
    private void OnDisable()
    {
        if (behavior != MovementBehavior.Default)
        {
            GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        }
    }

    void HandleRoomStateChange() {
        if (activeOnRoomColor == GameController.Instance.RoomState)
        {
            speed = Mathf.Abs(speed) * startingDirection;
            animator.speed = Mathf.Abs(speed * 28f);
            transform.localScale = new(Mathf.Sign(speed),1,1);
        }
        else
        {
            speed = Mathf.Abs(speed) * -startingDirection;
            animator.speed = Mathf.Abs(speed * 28f);
            transform.localScale = new(Mathf.Sign(speed),1,1);
        }
        StartCoroutine(WaitAndSwapBoost());
    }

    void FixedUpdate()
    {
        updateRaycastOrigins();

        if (boxCollider.enabled) 
        {
            CalculatePassengerMovement();
            MovePassengers();
        }
    }

    void Update()
    {
        if (player.PressedJump() && PlayerOnPlatform())
        {
            BoostPlayerJump();
        }
    }

    private void BoostPlayerJump()
    {
        StopAllCoroutines();
        if (
            
            boostAmmount != 0
        )
        {
            if(Mathf.Sign(player.GetVelocity().x) == boostDirection && player.IsMoving())
            {
                player.Boost(boostAmmount * boostDirection);
            }
            else if (!player.IsMoving())
            {
                player.Boost((boostAmmount/3) * boostDirection);
            }
        }
    }

     void MovePassengers()
    {
        foreach (PassengerMovement passenger in passengerMovements)
        {
            if (!passengerCollisionHandlers.ContainsKey(passenger.transform))
            {
                passengerCollisionHandlers.Add(passenger.transform, passenger.transform.GetComponent<MovementCollisionHandler>());

            }
                if (passengerCollisionHandlers[passenger.transform]) passengerCollisionHandlers[passenger.transform].Move(passenger.velocity, passenger.onPlatform, false);

        }
    }

    private IEnumerator WaitAndSwapBoost()
    {
        yield return new WaitForSeconds(0.05f);
        boostDirection = Mathf.Sign(speed);
    }

    void CalculatePassengerMovement()
    {
        
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovements = new List<PassengerMovement>();

        //Passenger on top of a platform not moving up====================================
        
            float rayLength = skinWidth * 2;
            Vector2 rayOrigin = raycastOrigins.topLeft;
            for (int i = 0; i < xRayCount; i++)
            {
                Debug.DrawRay(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.up, Color.cyan);
                RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.up, rayLength, passengerMask);
                if (collision.transform && !movedPassengers.Contains(collision.transform))
                {

                    float pushX = speed;

                    // collision.transform.Translate(new Vector3(pushX, pushY));
                    passengerMovements.Add(new PassengerMovement(collision.transform, new Vector3(pushX, 0), true, false));
                    movedPassengers.Add(collision.transform);

                }
            }
        
    }

    public bool PlayerOnPlatform()
    {
        float rayLength = skinWidth * 2;
        Vector2 rayOrigin = raycastOrigins.topLeft;
        for (int i = 0; i < xRayCount; i++)
            {
                RaycastHit2D collision = Physics2D.Raycast(rayOrigin + i * verticalRaySpacing * Vector2.right, Vector2.up, rayLength, passengerMask);
                if (collision.transform == player.transform)
                {
                    return true;
                }
            }
        return false;
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool onPlatform;
        public bool moveBeforePlatform;

        //Constructor
        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _onPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            onPlatform = _onPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
}
