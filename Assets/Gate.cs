using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Gate : MonoBehaviour
{
    public enum GateBehavior
    {
        OpenWhenNoEnemies,
    }

    public GateBehavior behavior = GateBehavior.OpenWhenNoEnemies;

    private Collider2D boxCollider;
    private string gateId;
    private Animator animator;
    private void OnEnable()
    {
        if (behavior == GateBehavior.OpenWhenNoEnemies)
        {
            GameController.Instance.OnEnemyKilled += CheckIfNoEnemies;
        }
    }
    private void OnDisable()
    {
        if (behavior == GateBehavior.OpenWhenNoEnemies)
        {
            GameController.Instance.OnEnemyKilled -= CheckIfNoEnemies;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gateId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";

        boxCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        if (GameController.Instance.CollectedObjects.Contains(gateId))
        {
            Destroy(gameObject);
            // Open();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        if (animator) animator.SetTrigger("Open");
        boxCollider.enabled = false;
    }

    public void Close()
    {

    }

    void CheckIfNoEnemies()
    {
        StartCoroutine(waitAndCheckIfEnemiesKilled());

    }

    private IEnumerator waitAndCheckIfEnemiesKilled()
    {
        yield return new WaitForEndOfFrame();
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        int enemyCount = allEnemies.Length;

        if (enemyCount <= 0)
        {
            Open();
            GameController.Instance.TagObjectStringAsCollected(gateId);
        }
    }
}
