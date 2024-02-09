using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{

    public Vector3[] localWaypoints;
    public Vector3[] globalWaypoints;

    [SerializeField]
    public bool shouldReverse = false;
    [SerializeField]
    public float waitTime = 0;
    [SerializeField][Range(0,2)]
    public float easeAmount = 0;
    [SerializeField]
    public float speed;
    public int fromWaypointIndex;
    public float percentBetweenWaypoints;
    public float nextMoveTime;

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    public virtual Vector3 CalculatePlatformMovement()
    {

        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

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
}
