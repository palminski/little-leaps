using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CameraControls : MonoBehaviour
{


    [SerializeField]
    public bool canMove = true;

    [SerializeField]
    public bool onlyUp = false;

    [SerializeField]
    public bool yAxisOnly = false;
    [SerializeField]
    public bool xAxisOnly = false;

    private Transform target;
    [SerializeField] private Vector3 offset;

    [Header("Easing")]
    [SerializeField] private float easeTimeX = 0.2f;

    [SerializeField] private float easeTimeY = 0.1f;

    [Header("Binding")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float boarderBuffer = 0.5f;



    private Vector3 velocity = Vector3.zero;

    [Header("Color")]
    [SerializeField] private float colorFadeSpeed = 2;
    [SerializeField] private float colorDarkness = 0.8f;
    [SerializeField] private float finalColorDarkness = 1f;

    private Color targetColor;
    private Camera cam;
    private Color colorForRS0;

    private Color colorForRS1;
    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }
    void Awake()
    {
        GameObject envObject = GameObject.Find("environment");
        if (!tilemap && envObject != null) tilemap = envObject.GetComponent<Tilemap>();

    }
    private IEnumerator Start()
    {
        colorForRS0 = GameController.ColorForPurple;
        colorForRS1 = GameController.ColorForGreen;
        cam = GetComponent<Camera>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            canMove = false;
        }

        // Vector3 startPosition = transform.position;
        // if (canMove && !onlyUp && !xAxisOnly) startPosition.y = target.position.y;
        // if (canMove && !onlyUp && !yAxisOnly) startPosition.x = target.position.x;
        // transform.position = canMove ? startPosition + offset : startPosition;



        if (GameController.Instance.RoomState == RoomColor.Purple)
        {
            targetColor = Color.Lerp(colorForRS0, Color.black, finalColorDarkness);
        }
        else
        {
            targetColor = Color.Lerp(colorForRS1, Color.black, finalColorDarkness);
        }
        cam.backgroundColor = targetColor;

        yield return null;
        GameObject envObject = GameObject.Find("environment");
        if (!tilemap && envObject != null) tilemap = envObject.GetComponent<Tilemap>();
        if (canMove)
        {
            // StartCoroutine(WaitAndSnapToPosition(target));
            SnapToPosition(target);
        }
    }

    void FixedUpdate()
    {
        if (canMove && target) EaseToTarget(target);
    }

    private void EaseToTarget(Transform target)
    {

        Vector3 targetPosition = target.position + offset;

        Vector3 xVector = new(transform.position.x, 0, 0);
        Vector3 targetXVector = new(targetPosition.x, 0, 0);

        Vector3 yVector = new(0, transform.position.y, 0);

        float yDifferentce = targetPosition.y - transform.position.y;
        // if (Mathf.Abs(yDifferentce) > 5)
        Vector3 targetYVector = (Mathf.Abs(yDifferentce) > 5) ? new(0, targetPosition.y, 0) : new(0, transform.position.y, 0);

        if (tilemap)
        {
            tilemap.CompressBounds();
            Camera camera = Camera.main;
            float verticalExtent = camera.orthographicSize + boarderBuffer * 2;
            float horizontalExtent = ((camera.orthographicSize + boarderBuffer) * camera.aspect) + boarderBuffer;

            Vector3 minPoint = onlyUp ? transform.position : tilemap.localBounds.min + new Vector3(horizontalExtent, verticalExtent, 0);
            Vector3 maxPoint = tilemap.localBounds.max - new Vector3(horizontalExtent, verticalExtent, 0);

            targetXVector = new(Mathf.Clamp(targetPosition.x, minPoint.x, maxPoint.x), 0, 0);
            targetYVector = new(0, Mathf.Clamp(targetPosition.y, minPoint.y, maxPoint.y), 0);
        }

        float xTarget = yAxisOnly ? transform.position.x : Vector3.SmoothDamp(xVector, targetXVector, ref velocity, easeTimeX).x;
        float yTarget = xAxisOnly ? transform.position.y : Vector3.SmoothDamp(yVector, targetYVector, ref velocity, easeTimeY).y;


        transform.position = new(xTarget, yTarget, transform.position.z);
    }

    private void Update()
    {
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, targetColor, colorFadeSpeed * Time.deltaTime);
    }

    public void SnapToPosition(Transform target)
    {
        Vector3 targetPosition = target.position + offset;

        Vector3 xVector = new(transform.position.x, 0, 0);
        Vector3 targetXVector = new(targetPosition.x, 0, 0);

        Vector3 yVector = new(0, transform.position.y, 0);
        Vector3 targetYVector = new(0, targetPosition.y, 0);

        if (tilemap)
        {
            tilemap.CompressBounds();
            Camera camera = Camera.main;
            float verticalExtent = camera.orthographicSize + boarderBuffer * 2;
            float horizontalExtent = ((camera.orthographicSize + boarderBuffer) * camera.aspect) + boarderBuffer;

            Vector3 minPoint = tilemap.localBounds.min + new Vector3(horizontalExtent, verticalExtent, 0);
            Vector3 maxPoint = tilemap.localBounds.max - new Vector3(horizontalExtent, verticalExtent, 0);

            targetXVector = new(Mathf.Clamp(targetPosition.x, minPoint.x, maxPoint.x), 0, 0);
            targetYVector = new(0, Mathf.Clamp(targetPosition.y, minPoint.y, maxPoint.y), 0);
        }

        float xTarget = yAxisOnly ? transform.position.x : targetXVector.x;
        float yTarget = xAxisOnly ? transform.position.y : targetYVector.y;

        transform.position = new(xTarget, yTarget, transform.position.z);
    }

    private void HandleRoomStateChange()
    {
        if (cam == null) return;
        if (GameController.Instance.RoomState == RoomColor.Purple)
        {
            cam.backgroundColor = Color.Lerp(colorForRS0, Color.black, colorDarkness);
            targetColor = Color.Lerp(colorForRS0, Color.black, finalColorDarkness);
        }
        else
        {
            cam.backgroundColor = Color.Lerp(colorForRS1, Color.black, colorDarkness);
            targetColor = Color.Lerp(colorForRS1, Color.black, finalColorDarkness);
        }
    }

    private IEnumerator WaitAndSnapToPosition(Transform target)
    {
        yield return new WaitForEndOfFrame();

        SnapToPosition(transform);
    }

    public void UnlockCameraAfterDelay(float delayTime)
    {
        if (canMove) return;
        StartCoroutine(WaitAndUnlock(delayTime));
    }
    private IEnumerator WaitAndUnlock(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        canMove = true;
    }
}
