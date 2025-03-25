using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldFollowPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float magnetRadius = 1;
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private int pointValue = 100;

    [SerializeField] private int chargeValue = 1;

    public ParticleSystem coinParticleSystem;

    [SerializeField] private GameObject collectionParticle;
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

    public void CollectForPoints(string pickupId)
    {
        GameController.Instance.UpdateSectionChips(1);
        int pointsToAward = pointValue * GameController.Instance.SectionChips;
        GameController.Instance.UpdateInstability(1);

        if (AudioController.Instance != null) AudioController.Instance.PlayPickupNoise();
        
        GameController.Instance.AddToScore(pointsToAward);
        if (GameController.Instance.TimerMoving) GameController.Instance.AddToTimer(30);
        if (player) GameController.Instance.ShowPointCounter(pointsToAward, player.transform.position + new Vector3(0,1,0), false);
        GameController.Instance.ChangeCharge(chargeValue);
        if (coinParticleSystem)
        {
            coinParticleSystem.transform.SetParent(null);
            coinParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        } 
        GameController.Instance.PullFromPool(collectionParticle,transform.position);
        GameController.Instance.TagObjectStringAsCollected(pickupId);
        GameController.Instance.TagObjectStringAsCollectedForSession(pickupId);
        Destroy(gameObject);
    }
}