using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [Header("Health Points Damage")]
    [SerializeField] private int pointValue = 100;
    [SerializeField] private float bounceMultiplier = 1;
    [SerializeField] private int health = 1;
    [SerializeField] private bool invincible = false;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject[] objectsToSpawnOnDeath;

    [Header("Vulnerabilities")]
    [SerializeField] private bool canSwapColors = false;
    [SerializeField] private bool vulnerableFromTop;
    [SerializeField] private bool vulnerableFromSide;
    
    [Header("Purple Vulnerabilities")]
    [SerializeField] private bool purpleVulnerableFromTop;
    [SerializeField] private bool purpleVulnerableFromSide;

    [Header("Green Vulnerabilities")]
    [SerializeField] private bool greenVulnerableFromTop;
    [SerializeField] private bool greenVulnerableFromSide;


    private GameObject player;
    private GameObject playerAttack;

    



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttack = GameObject.FindGameObjectWithTag("PlayerAttack");
    }

    private void OnEnable()
    {
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
            UpdateVulnerabilities();
        }
    }
    private void OnDisable()
    {
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        }
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        // - Check if enemy can hurt player
        if (hitCollider.gameObject == player)
        {
            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider = GetComponent<Collider2D>();

            float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            float enemyTop = transform.position.y + collider.offset.y + collider.bounds.size.y / 2;
            if (vulnerableFromTop && playerBottom > enemyTop )
            {
                return;
            }

            if (vulnerableFromSide && playerBottom <= enemyTop && hitPlayer.IsDashing())
            {
                return;
            }

            if (!hitPlayer.IsInvincible())
            {
                int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
                hitPlayer.Damage(1, directionToShove);
                hitPlayer.Shove(directionToShove);
            }
        }

        // - Check if player can hurt enemy
        if (hitCollider.gameObject == playerAttack)
        {
            
            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider;
            collider = GetComponent<Collider2D>();

            float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            float enemyTop = transform.position.y + collider.offset.y + collider.bounds.size.y / 2;


            if (vulnerableFromTop && playerBottom > enemyTop && hitPlayer.IsDashing())
            {
                
                hitPlayer.ResetCoyoteTime();
                player.transform.position = new Vector3(player.transform.position.x, enemyTop,player.transform.position.z);
                hitPlayer.Bounce(bounceMultiplier);
                

                DamageEnemy(1, true);
            }
            else if (vulnerableFromTop && playerBottom > enemyTop)
            {
                
                hitPlayer.ResetCoyoteTime();
                player.transform.position = new Vector3(player.transform.position.x, enemyTop,player.transform.position.z);
                hitPlayer.Bounce(1);
                

                DamageEnemy();
            }

            if (vulnerableFromSide && playerBottom <= enemyTop && hitPlayer.IsDashing())
            {
                hitPlayer.ResetCoyoteTime();

                DamageEnemy();
            }
        }
    }


    public void DamageEnemy(int damage = 1, bool IsDashing = false)
    {
        if (invincible) return;
        health -= damage;
        if (health <= 0)
        {
            KillEnemy(IsDashing);
        }
    }

    public void KillEnemy(bool bonusPoints = false)
    {
        print("here");
        // GameController.Instance.AddToScore(pointValue);
        if (blood) GameController.Instance.PullFromPool(blood, transform.position);
        GameLight light = GetComponentInChildren<GameLight>();
        if (light)
        {
            light.transform.SetParent(null);
            light.Fade();
        }
        foreach (GameObject objectToSpawn in objectsToSpawnOnDeath) {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        GameController.Instance.AddToScore(pointValue);
        // if (bonusPoints) GameController.Instance.AddToScore(pointValue);
    }


    private void HandleRoomStateChange()
    {
        UpdateVulnerabilities();
    }

    private void UpdateVulnerabilities()
    {
        if (GameController.Instance.RoomState == RoomColor.Purple)
        {
            vulnerableFromTop = purpleVulnerableFromTop;
            vulnerableFromSide = purpleVulnerableFromSide;
        }
        else
        {
            vulnerableFromTop = greenVulnerableFromTop;
            vulnerableFromSide = greenVulnerableFromSide;
        }
    }
}
