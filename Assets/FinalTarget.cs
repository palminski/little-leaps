using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class FinalTarget : MonoBehaviour
{
    private Transform playerTransform;
    private Player playerScript;

    public GameObject finalObject;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = playerTransform.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D hitCollider)
    {
        if (finalObject != null && hitCollider.gameObject == playerTransform.gameObject && playerScript.IsGrounded())
        {

            if (playerScript) playerScript.SetInputEnabled(false);
            Instantiate(finalObject, playerTransform.position, playerTransform.rotation);
            if (SteamManager.Initialized)
            {
                if (GameController.Instance.SessionCollectedObjects.Contains("lv_1_started"))
                {
                    SteamUserStats.SetAchievement("ACH_ONE_QUARTER");
                    SteamUserStats.StoreStats();
                }

            }
        }
    }
}
