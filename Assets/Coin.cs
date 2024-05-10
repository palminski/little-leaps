using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    [SerializeField]
    private int pointValue = 100;

    [SerializeField]
    private int chargeValue = 1;

    [SerializeField]
    private float magnetRadius = 1;
    [SerializeField]
    private float maxSpeed = 2;

    [SerializeField]
    private GameObject collectionParticle;

    public ParticleSystem coinParticleSystem;

    private string coinId;
    
    // Start is called before the first frame update
    void Start()
    {
        coinId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(coinId)) Destroy(gameObject);
        
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (ShouldMoveTowardsPlayer())
        {
            Vector2 vectorToPlayer = player.transform.position - transform.position;
            float distanceToPlayer = vectorToPlayer.magnitude;
            Vector2 moveDirection = vectorToPlayer.normalized;
            
            float speed = (1-distanceToPlayer/magnetRadius) * maxSpeed;
            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            GameController.Instance.AddToScore(pointValue);
            GameController.Instance.ChangeCharge(chargeValue);
            if (coinParticleSystem)
            {
                coinParticleSystem.transform.SetParent(null);
                coinParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            } 
            GameController.Instance.PullFromPool(collectionParticle,transform.position);
            GameController.Instance.TagObjectStringAsCollected(coinId);
            Destroy(gameObject);
        }
    }

    private bool ShouldMoveTowardsPlayer() {
        if (Vector2.Distance(transform.position, player.transform.position) <= magnetRadius) return true;
        return false;
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
