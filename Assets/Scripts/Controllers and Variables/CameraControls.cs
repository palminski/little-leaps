using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CameraControls : MonoBehaviour
{


    [SerializeField]
    public bool canMove = true;
    private Transform target;
    [SerializeField] private Vector3 offset;

    [Header("Easing")]
    [SerializeField] private float easeTimeX = 0.2f;

    [SerializeField] private float easeTimeY = 0.1f;

    [Header("Binding")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float boarderBuffer = 0.5f;

    [Header("Only Up")]
    [SerializeField] public bool onlyUp = false;

    [SerializeField] private float distanceAllowedBack = 0.5f;

    private float highestPoint;

    private Vector3 velocity = Vector3.zero;

    [Header("Color")]
     private float colorFadeSpeed = 2;
     private float colorDarkness = 0.8f;
    private Camera cam;
    private Color colorForRS0;

    private Color colorForRS1;
    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }
    void Start()
    {
        colorForRS0 = GameController.ColorForPurple;
        colorForRS1 = GameController.ColorForGreen;
        cam = GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (canMove && !onlyUp) transform.position = target.position + offset;
        highestPoint = transform.position.y;
        if (!tilemap) tilemap = GameObject.Find("environment").GetComponent<Tilemap>();

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
            float verticalExtent = camera.orthographicSize + boarderBuffer * 2;
            float horizontalExtent = ((camera.orthographicSize + boarderBuffer) * camera.aspect) + boarderBuffer;

            Vector3 minPoint = tilemap.localBounds.min + new Vector3(horizontalExtent, verticalExtent, 0);
            Vector3 maxPoint = tilemap.localBounds.max - new Vector3(horizontalExtent, verticalExtent, 0);

            xTarget = Mathf.Clamp(xTarget, minPoint.x, maxPoint.x);
            yTarget = Mathf.Clamp(yTarget, minPoint.y, maxPoint.y);
        }
        if (onlyUp) {
            highestPoint = Mathf.Max(highestPoint, transform.position.y);
            transform.position = new(transform.position.x, Mathf.Max(yTarget, highestPoint-distanceAllowedBack) , transform.position.z);
            return;
        }
        transform.position = new(xTarget, yTarget, transform.position.z);
    }
    
    private void Update()
    {
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, Color.black, colorFadeSpeed*Time.deltaTime);
    }

    public void SnapToPosition(Transform target)
    {
        transform.position = target.position + offset;
    }

    private void HandleRoomStateChange()
    {
        if (GameController.Instance.RoomState == RoomColor.Purple) {
            cam.backgroundColor = Color.Lerp(colorForRS0, Color.black, colorDarkness);
            
        }
        else {
            cam.backgroundColor = Color.Lerp(colorForRS1, Color.black, colorDarkness);
        }
    }

}
