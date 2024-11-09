using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaypointMovement : MonoBehaviour
{
    private string doorId;
    public enum MovementBehavior
    {
        Default,
        StopOnInactiveColor,
        ColorCorespondsToWaypoint,
        OpenOnTrigger,
        MoveOnPreseure,
    }


    public MovementBehavior behavior = MovementBehavior.Default;

    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    [SerializeField]
    public bool shouldReverse = false;
    [SerializeField]
    public float waitTime = 0;
    [SerializeField]
    [Range(0, 2)]
    public float easeAmount = 0;
    [SerializeField]
    private float speed;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextMoveTime;

    [SerializeField]
    private TriggerEvent OpenEvent;

    private bool shouldMove = false;
    private bool shouldMoveAgain = false;

    private PlatformController platformController;
    private FallthroughSolid fallthroughSolid;

    [Header("Color Toggle Settings")]

    [SerializeField]
    private RoomColor activeOnRoomColor;



    //===================================================================================================
    // Start is called before the first frame update
    void Start()
    {
        doorId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (behavior == MovementBehavior.OpenOnTrigger)
        {
            if (GameController.Instance.CollectedObjects.Contains(doorId))
            {
                shouldMove = true;
            }
        }

        percentBetweenWaypoints = 1;
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
        SetInitialPosition();
        if (behavior == MovementBehavior.MoveOnPreseure)
        {
            platformController = GetComponent<PlatformController>();
            fallthroughSolid = GetComponent<FallthroughSolid>();
        }
    }

    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        if (OpenEvent)
        {
            OpenEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        if (OpenEvent)
        {
            OpenEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    public void SetInitialPosition()
    {
        if (behavior == MovementBehavior.Default)
        {

        }
        else if (behavior == MovementBehavior.StopOnInactiveColor)
        {

        }
        else if (behavior == MovementBehavior.ColorCorespondsToWaypoint)
        {

        }
        else
        {

        }
    }

    public virtual Vector3 CalculatePlatformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        if (behavior == MovementBehavior.Default)
        {
            return MovementDefault();
        }
        else if (behavior == MovementBehavior.StopOnInactiveColor)
        {
            return MovementStopOnInactiveColor();
        }
        else if (behavior == MovementBehavior.ColorCorespondsToWaypoint)
        {
            return MovementColorCorespondsToWaypoint();
        }
        else if (behavior == MovementBehavior.MoveOnPreseure)
        {
            return MoveOnPreseure();
        }
        else
        {
            return MovementOpenOnTrigger();
        }
    }

    private Vector3 MovementDefault()
    {
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += speed / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (shouldReverse)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }
        return newPos - transform.position;
    }

    private Vector3 MovementStopOnInactiveColor()
    {

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += (activeOnRoomColor == GameController.Instance.RoomState ? speed : 0) / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (shouldReverse)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }
        return newPos - transform.position;
    }

    private Vector3 MovementColorCorespondsToWaypoint()
    {

        fromWaypointIndex = activeOnRoomColor == GameController.Instance.RoomState ? 0 : 1;
        int toWaypointIndex = activeOnRoomColor == GameController.Instance.RoomState ? 1 : 0;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += speed / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 1;

        }
        return newPos - transform.position;
    }

    private Vector3 MovementOpenOnTrigger()
    {

        fromWaypointIndex = shouldMoveAgain ? 1 :(shouldMove ? 0 : 1);
        int toWaypointIndex = shouldMoveAgain ? 2 :(shouldMove ? 1 : 0);
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += speed / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 1;

        }
        return newPos - transform.position;
    }

    private Vector3 MoveOnPreseure()
    {
        fromWaypointIndex = platformController.PassengerOnPlatform() && (fallthroughSolid == null || fallthroughSolid.IsPlayerAbove()) ? 0 : 1;
        int toWaypointIndex = platformController.PassengerOnPlatform() && (fallthroughSolid == null || fallthroughSolid.IsPlayerAbove()) ? 1 : 0;
        float distBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += speed / distBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 1;

        }
        return newPos - transform.position;
    }

    public float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.magenta;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawSphere(globalWaypointPosition, size);
            }
        }
    }

    void HandleRoomStateChange()
    {
        if (behavior == MovementBehavior.ColorCorespondsToWaypoint)
        {

            int toWaypointIndex = activeOnRoomColor == GameController.Instance.RoomState ? 1 : 0;
            float distBetweenWaypoints = Vector3.Distance(transform.position, globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints = 1 - percentBetweenWaypoints;

        }
    }

    void OnEventRaised()
    {
        if (shouldMove) return;
        GameController.Instance.TagObjectStringAsCollected(doorId);
        percentBetweenWaypoints = 1 - percentBetweenWaypoints;
        shouldMove = true;
    }

    public void TriggerShouldMove()
    {
        
        percentBetweenWaypoints = 1 - percentBetweenWaypoints;
        
        shouldMove = true;
        shouldMoveAgain = true;
    }
}
