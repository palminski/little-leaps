using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float magnetRadius = 1;
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private float maxDistance = 5f;

    [SerializeField] public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 targetPosition = player.transform.position + offset * (-player.transform.localScale.x);
        Vector3 targetPosition = player.transform.position;
        Vector2 vectorToPlayer = targetPosition - transform.position;
        float distanceToPlayer = vectorToPlayer.magnitude -1f;
        Vector2 moveDirection = vectorToPlayer.normalized;

        
        float speed = (distanceToPlayer / magnetRadius) * maxSpeed;
        transform.position += (Vector3)(speed * Time.deltaTime * moveDirection);
    }
}
