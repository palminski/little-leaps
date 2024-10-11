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

    private bool shouldMoveTowardsPlayer;
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        coinId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(coinId)) Destroy(gameObject);
        
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player && player.activeSelf && Vector2.Distance(transform.position, player.transform.position) <= magnetRadius) shouldMoveTowardsPlayer = true;
        if (player && shouldMoveTowardsPlayer)
        {
            Vector2 vectorToPlayer = player.transform.position - transform.position;
            float distanceToPlayer = vectorToPlayer.magnitude;
            Vector2 moveDirection = vectorToPlayer.normalized;
            
            speed = Mathf.Max(speed,((1-distanceToPlayer/magnetRadius) * maxSpeed));
            speed += Time.deltaTime * speed;
            transform.position += (Vector3)(speed * Time.deltaTime * moveDirection);
        }
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            GameController.Instance.AddToScore(pointValue);
            GameController.Instance.ShowPointCounter(pointValue, player.transform.position + new Vector3(0,1,0), false);
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
        if (player && player.activeSelf && Vector2.Distance(transform.position, player.transform.position) <= magnetRadius) return true;
        return false;
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
