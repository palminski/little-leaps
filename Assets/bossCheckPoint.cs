using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossCheckPoint : MonoBehaviour
{
    [SerializeField] private float bossPoint;

    private RiseHazard bossHazard;
    private Player player;
    [SerializeField] private bool stopBossHazard = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        bossHazard = FindObjectOfType<RiseHazard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossHazard && bossHazard.transform.position.y > (transform.position.y - Mathf.Abs(bossPoint)))
        {
            player.SetPlayerSpawnPointToTransform(transform);
            if (stopBossHazard)
            {
                bossHazard.shouldStop = true;
            }
            Destroy(gameObject);

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        float size = 0.3f;

        
            Vector3 globalWaypointPosition = new(transform.position.x, transform.position.y - Mathf.Abs(bossPoint), 0);
            Gizmos.DrawSphere(globalWaypointPosition, size);
    }
}
