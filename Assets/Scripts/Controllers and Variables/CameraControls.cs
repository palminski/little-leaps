using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CameraControls : MonoBehaviour
{


    [SerializeField]
    public bool canMove = true;
    
    private Transform target;

    [SerializeField]
    private Vector3 offset;

    [Header("Easing")]
    [SerializeField]
    private float easeTimeX = 0.2f;
    [SerializeField]
    private float easeTimeY = 0.1f;

    [Header("Binding")]
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private float boarderBuffer = 0.5f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (canMove) transform.position = target.position + offset;
    }

    void FixedUpdate()
    {
        if (canMove) EaseToTarget(target);
    }

    private void EaseToTarget(Transform target)
    {
        Vector3 targetPosition = target.position + offset;

        Vector3 xVector = new(transform.position.x, 0, 0);
        Vector3 targetXVector = new(targetPosition.x, 0, 0);

        Vector3 yVector = new(0, transform.position.y, 0);
        Vector3 targetYVector = new(0, targetPosition.y, 0);

        float xTarget = Vector3.SmoothDamp(xVector, targetXVector, ref velocity, easeTimeX).x;
        float yTarget = Vector3.SmoothDamp(yVector, targetYVector, ref velocity, easeTimeY).y;

        if (tilemap)
        {
            tilemap.CompressBounds();
            Camera camera = Camera.main;
            float verticalExtent = camera.orthographicSize + boarderBuffer;
            float horizontalExtent = (verticalExtent * camera.aspect) + boarderBuffer;

            Vector3 minPoint = tilemap.localBounds.min + new Vector3(horizontalExtent, verticalExtent, 0);
            Vector3 maxPoint = tilemap.localBounds.max - new Vector3(horizontalExtent, verticalExtent, 0);

            xTarget = Mathf.Clamp(xTarget, minPoint.x, maxPoint.x);
            yTarget = Mathf.Clamp(yTarget, minPoint.y, maxPoint.y);
        }

        transform.position = new(xTarget, yTarget, transform.position.z);
    }

    public void SnapToPosition(Transform target)
    {
        transform.position = target.position + offset;
    }

}
