using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float magnetRadius = 1;
    [SerializeField] private float maxSpeed = 2;

    public int offset;

    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void AssignToPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.GetComponent<Player>().startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            targetPosition = player.transform.position;
        }
        Vector2 vectorToPlayer = targetPosition - transform.position;
        float distanceToPlayer = vectorToPlayer.magnitude - 1f * offset;
        Vector2 moveDirection = vectorToPlayer.normalized;


        float speed = (distanceToPlayer / magnetRadius) * maxSpeed;
        transform.position += (Vector3)(speed * Time.deltaTime * moveDirection);
    }
}
