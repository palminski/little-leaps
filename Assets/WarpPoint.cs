using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    private Vector3[] globalTargetPoints;
    private float nextWarpTime;

    [Header("Movement Settings")]
    [SerializeField]
    private Vector3[] targetPoints;
    [SerializeField]
    private bool isToggleable = true;
    [SerializeField]
    private float warpDelay = 5f;

    [Header("Warp Settings")]
    [SerializeField]
    private LayerMask warpableLayers;
    [SerializeField]
    private float radius = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        globalTargetPoints = new Vector3[targetPoints.Length];
        for (int i = 0; i < targetPoints.Length; i++)
        {
            globalTargetPoints[i] = targetPoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isToggleable)
        {
            if (Time.time > nextWarpTime)
            {
                Warp();
                nextWarpTime = Time.time + warpDelay;
            }
        }
    }

    private void OnEnable()
    {
        if (isToggleable) GameController.Instance.OnRoomStateChanged += Warp;
    }
    private void OnDisable()
    {
        if (isToggleable) GameController.Instance.OnRoomStateChanged -= Warp;
    }

    void Warp()
    {
        if (globalTargetPoints.Length != 2) return;
        Vector3 targetPosition = transform.position == globalTargetPoints[0] ? globalTargetPoints[1] : globalTargetPoints[0];

        Collider2D[] passengers = Physics2D.OverlapCircleAll(transform.position, radius, warpableLayers);


        foreach (Collider2D passenger in passengers)
        {
            //break if the passenger is a child of a enemy. 
            //This uses the bitwise and operator to determine if the layer is in a layer mask
            if (passenger.transform.parent != null && warpableLayers == (warpableLayers & (1 << passenger.transform.parent.gameObject.layer))) continue;
            Vector3 offset = passenger.transform.position - transform.position;
            passenger.transform.position = targetPosition + offset;

        }
        transform.position = targetPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (targetPoints.Length != 2 || Application.isPlaying) return;

        Gizmos.DrawSphere(targetPoints[0] + transform.position, 0.3f);
        Gizmos.DrawSphere(targetPoints[1] + transform.position, 0.3f);
        Gizmos.DrawLine(targetPoints[0] + transform.position, targetPoints[1] + transform.position);
    }
}
