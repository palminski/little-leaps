using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Health Points Damage")]
    [SerializeField] private int pointValue = 100;
    [SerializeField] private float bounceMultiplier = 1;
    [SerializeField] public int health = 1;
    [SerializeField] public bool invincible = false;
    [SerializeField] public GameObject blood;
    [SerializeField] private GameObject[] objectsToSpawnOnDeath;
    [SerializeField] private bool canRespwan = false;

    [Header("Vulnerabilities")]
    [SerializeField] public bool canSwapColors = false;
    [SerializeField] public bool vulnerableFromTop;
    [SerializeField] private bool vulnerableFromSide;

    [Header("Purple Vulnerabilities")]
    [SerializeField] private bool purpleVulnerableFromTop;
    [SerializeField] private bool purpleVulnerableFromSide;

    [Header("Green Vulnerabilities")]
    [SerializeField] private bool greenVulnerableFromTop;
    [SerializeField] private bool greenVulnerableFromSide;

    [SerializeField] private float minimumPlayerFallAmmountToCountAsKill = 0.005f;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject playerAttack;

    public bool canDamagePlayer = false;
    private string enemyId;

    public bool wasKilled = false;

    private Vector3 lastPosition;

    [SerializeField] private float maxImunityTime = 1f;
    public float immunityTime = 0;

    public bool shouldStartHarmless = false;


    void Update()
    {
        if (immunityTime > 0) immunityTime -= Time.deltaTime;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(enemyId))
        {
            if (!canRespwan) Destroy(gameObject);
            pointValue = 0;
        }

        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (playerAttack == null) playerAttack = GameObject.FindGameObjectWithTag("PlayerAttack");

        if (shouldStartHarmless)
        {
            canDamagePlayer = false;
            StartCoroutine(WaitAndBecomeDangerous());
        }
    }

    private void OnEnable()
    {
        canDamagePlayer = true;
        wasKilled = false;
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
            UpdateVulnerabilities();
        }
    }
    private void OnDisable()
    {
        canDamagePlayer = false;
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        }
    }

    void LateUpdate()
    {
        lastPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        // - Check if enemy can hurt player
        if (canDamagePlayer && hitCollider.gameObject == player)
        {
            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider = GetComponent<Collider2D>();

            // float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            bool playerWasGoingDown = hitPlayer.GetLastPosition().y > hitPlayer.transform.position.y + minimumPlayerFallAmmountToCountAsKill;

            // float enemyTop = lastPosition.y + collider.offset.y + collider.bounds.size.y / 2;
            if (vulnerableFromTop && playerWasGoingDown)
            {
                return;
            }

            if (vulnerableFromSide && hitPlayer.IsDashingSideways())
            {
                return;
            }

            if (!hitPlayer.IsInvincible())
            {
                int directionToShove = (hitPlayer.GetLastPosition().x > lastPosition.x) ? 1 : -1;
                hitPlayer.Damage(1, directionToShove);
                hitPlayer.Shove(directionToShove);
            }
        }

        // - Check if player can hurt enemy
        if (hitCollider.gameObject == playerAttack && immunityTime <= 0)
        {

            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider;
            collider = GetComponent<Collider2D>();

            // float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            float enemyTop = lastPosition.y + collider.offset.y + collider.bounds.size.y / 2;

            bool playerWasGoingDown = hitPlayer.GetLastPosition().y > hitPlayer.transform.position.y + minimumPlayerFallAmmountToCountAsKill;

            if (vulnerableFromTop && playerWasGoingDown && hitPlayer.IsDashing())
            {

                hitPlayer.ResetCoyoteTime();

                player.transform.position = new Vector3(player.transform.position.x, enemyTop + 0.5f, player.transform.position.z);
                hitPlayer.Bounce(bounceMultiplier);



                DamageEnemy(1, true);
            }
            else if (vulnerableFromTop && playerWasGoingDown)
            {

                hitPlayer.ResetCoyoteTime();
                player.transform.position = new Vector3(player.transform.position.x, enemyTop + 0.5f, player.transform.position.z);
                hitPlayer.Bounce(1);


                DamageEnemy();
            }

            if (vulnerableFromSide && hitPlayer.IsDashingSideways())
            {
                // hitPlayer.ResetCoyoteTime();
                hitPlayer.RefreshDashMoves();
                DamageEnemy(1, true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D hitCollider)
    {
        // - Check if player can hurt enemy
        if (vulnerableFromSide && hitCollider.gameObject == playerAttack)
        {

            Player hitPlayer = player.GetComponent<Player>();
            Collider2D collider;
            collider = GetComponent<Collider2D>();

            // float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            float enemyTop = lastPosition.y + collider.offset.y + collider.bounds.size.y / 2;


            if (vulnerableFromSide && hitPlayer.IsDashingSideways())
            {
                // hitPlayer.ResetCoyoteTime();
                hitPlayer.RefreshDashMoves();
                DamageEnemy();
            }
        }
    }


    public virtual void DamageEnemy(int damage = 1, bool IsDashing = false)
    {
        if (invincible || immunityTime > 0) return;
        immunityTime = maxImunityTime;
        if (AudioController.Instance != null) AudioController.Instance.PlayEnemyKilled();
        health -= damage;
        if (blood) GameController.Instance.PullFromPool(blood, transform.position);

        if (health <= 0)
        {
            KillEnemy(IsDashing);
        }
    }

    public void KillEnemy(bool bonusPoints = false)
    {
        if (wasKilled) return;
        wasKilled = true;
        int bonusPointsToAdd = GameController.Instance.CollectedObjects.Contains(enemyId) ? 0 : 100;
        // GameController.Instance.AddToScore(pointValue);
        GameLight light = GetComponentInChildren<GameLight>();
        if (light)
        {
            light.transform.SetParent(null);
            light.Fade();
        }
        foreach (GameObject objectToSpawn in objectsToSpawnOnDeath)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
        GameController.Instance.TagObjectStringAsCollected(enemyId);
        if (canRespwan)
        {

            GameController.Instance.AddInactiveEnemy(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
        GameController.Instance.InvokeEnemyKilled();

        int pointsToAdd = pointValue;
        if (bonusPoints)
        {
            pointsToAdd += bonusPointsToAdd;
        }
        GameController.Instance.AddToScore(pointsToAdd);
        if (pointValue > 0) GameController.Instance.ShowPointCounter(pointsToAdd, transform.position);
        pointValue = 0;
    }


    public void HandleRoomStateChange()
    {
        UpdateVulnerabilities();
    }

    public void UpdateVulnerabilities()
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

    IEnumerator WaitAndBecomeDangerous()
    {
        yield return new WaitForSeconds(0.15f);
        canDamagePlayer = true;
    }
    
}
