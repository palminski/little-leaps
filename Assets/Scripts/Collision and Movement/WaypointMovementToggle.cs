using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovementToggle : WaypointMovement
{



    [SerializeField]
    private RoomColor activeOnRoomColor;

    [SerializeField]
    private bool toggleToStopStart = true;
    // Start is called before the first frame update

    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }
    public override Vector3 CalculatePlatformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        if (toggleToStopStart)
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
        else
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
    }

    void HandleRoomStateChange() {
        if (!toggleToStopStart) {
            int toWaypointIndex = activeOnRoomColor == GameController.Instance.RoomState ? 1 : 0;
            float distBetweenWaypoints = Vector3.Distance(transform.position, globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints = 1 - percentBetweenWaypoints;
        }
    }
}
