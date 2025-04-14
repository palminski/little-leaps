using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiseHazard : MonoBehaviour
{
    [SerializeField] private float raiseSpeed = 0.05f;
    [SerializeField] private float slowSpeed = 0.05f;
    [HideInInspector] public bool shouldStop = false;

    private Player player;
    private GameObject playerGO;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO) player = playerGO.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {   
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        if (!shouldStop && playerGO != null && (!playerGO.activeSelf || player.IsInvincible()))
        {
            transform.position = new(transform.position.x, cameraBottom - 0.75f, transform.position.z);
            return;
        }

        if (shouldStop && raiseSpeed > 0)
        {
            raiseSpeed = Mathf.Max(0, (raiseSpeed - slowSpeed * Time.deltaTime));
        }
        if (cameraBottom - 0.75f > transform.position.y)
        {
            transform.position = new(transform.position.x, cameraBottom - 0.75f, transform.position.z);
        }
        transform.Translate(0, raiseSpeed * Time.deltaTime, 0);

    }

}
