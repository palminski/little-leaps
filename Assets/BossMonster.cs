using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Enemy
{
    [SerializeField] private GameObject[] retainers;
    private List<GameObject> spawnedRetainers = new List<GameObject>();

    [SerializeField] private GameObject crown;
    [SerializeField] private bool randomiseSpeed = true;

    [SerializeField] private GameObject[] corpseEnemies;

    void OnEnable()
    {

        if (crown != null) crown.SetActive(false);

        GameController.Instance.OnEnemyKilled += CheckIfNoEnemies;

        canDamagePlayer = true;
        wasKilled = false;
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
            UpdateVulnerabilities();
        }
    }
    void OnDisable()
    {
        GameController.Instance.OnEnemyKilled -= CheckIfNoEnemies;

        canDamagePlayer = false;
        if (canSwapColors)
        {
            GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        }
    }
    public override void DamageEnemy(int damage = 1, bool IsDashing = false)
    {
        if (invincible || immunityTime > 0) return;
        bool startRight = Random.value > 0.5f;
        spawnedRetainers.Clear();
        if (health > 1)
        {
            foreach (GameObject retainer in retainers)
            {
                GameObject newRetainer = Instantiate(retainer, transform.position, transform.rotation);
                if (blood) GameController.Instance.PullFromPool(blood, transform.position);

                spawnedRetainers.Add(newRetainer);
                Enemy enemy = newRetainer.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.player = player;
                    enemy.playerAttack = playerAttack;
                }

                EnemyPatrol enemyPatrol = newRetainer.GetComponent<EnemyPatrol>();
                if (enemyPatrol != null)
                {
                    enemyPatrol.startRight = startRight;
                    if (randomiseSpeed) enemyPatrol.speed = Random.Range(0.035f, 0.05f);
                }

                EnemyFloatAimless enemyFloatAimless = newRetainer.GetComponent<EnemyFloatAimless>();
                if (enemyFloatAimless != null)
                {
                    enemyFloatAimless.startDirection = startRight ? new(1, Random.Range(-0.2f, -0.4f)) : new(-1, Random.Range(-0.2f, -0.4f));
                    if (randomiseSpeed) enemyFloatAimless.speed = Random.Range(0.035f, 0.07f);
                }
                startRight = !startRight;
            }
            base.DamageEnemy();
            invincible = true;
            StartCoroutine(waitAndEnableCrown());
        }
        else
        {
            if (blood) GameController.Instance.PullFromPool(blood, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f));
            if (blood) GameController.Instance.PullFromPool(blood, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f));
            if (blood) GameController.Instance.PullFromPool(blood, transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f));
            base.DamageEnemy();





            foreach (GameObject corpseEnemy in corpseEnemies)
            {
                GameObject newEnemy = Instantiate(corpseEnemy, transform.position, transform.rotation);
                // if (blood) GameController.Instance.PullFromPool(blood, transform.position);


                Enemy enemy = newEnemy.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.player = player;
                    enemy.playerAttack = playerAttack;
                    enemy.shouldStartHarmless = true;
                }

                EnemyPatrol enemyPatrol = newEnemy.GetComponent<EnemyPatrol>();
                if (enemyPatrol != null)
                {
                    enemyPatrol.startRight = startRight;
                    if (randomiseSpeed) enemyPatrol.speed = Random.Range(0.035f, 0.05f);
                }

                EnemyFloatAimless enemyFloatAimless = newEnemy.GetComponent<EnemyFloatAimless>();
                if (enemyFloatAimless != null)
                {
                    enemyFloatAimless.startDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    if (randomiseSpeed) enemyFloatAimless.speed = Random.Range(0.035f, 0.07f);
                }
                startRight = !startRight;
            }

        }


    }

    void CheckIfNoEnemies()
    {
        StartCoroutine(waitAndCheckIfEnemiesKilled());

    }

    IEnumerator waitAndCheckIfEnemiesKilled()
    {
        yield return new WaitForEndOfFrame();
        foreach (GameObject enemy in spawnedRetainers)
        {
            if (enemy != null) yield break;
        }
        invincible = false;
        vulnerableFromTop = true;
        if (crown != null) crown.SetActive(false);

    }

    IEnumerator waitAndEnableCrown()
    {
        yield return new WaitForSeconds(0.1f);
        if (crown != null) crown.SetActive(true);
        HazardToPlayer hazardToPlayer = crown.GetComponent<HazardToPlayer>();

        vulnerableFromTop = false;

        if (hazardToPlayer != null)
        {
            hazardToPlayer.SetCanDamagePlayer(false);
            yield return new WaitForSeconds(0.4f);
            hazardToPlayer.SetCanDamagePlayer(true);
        }


    }



    void OnDestroy()
    {


        Enemy[] allEnemies = FindObjectsOfType<Enemy>();


        int enemyCount = allEnemies.Length;
        if (enemyCount <= 0)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                CameraControls cameraControls = camera.GetComponent<CameraControls>();
                if (cameraControls != null)
                {
                    cameraControls.UnlockCameraAfterDelay(1f);

                }
            }
        }


        // Camera camera = Camera.main;
        // if (camera != null)
        // {
        //     CameraControls cameraControls = camera.GetComponent<CameraControls>();
        //     if (cameraControls != null)
        //     {
        //         cameraControls.UnlockCameraAfterDelay(1f);

        //     }
        // }
    }
}
