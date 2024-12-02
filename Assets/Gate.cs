using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Gate : MonoBehaviour
{
    public enum GateBehavior
    {
        None,
        CloseOnTimer,
        OpenWhenNoEnemies,
    }

    public GateBehavior behavior = GateBehavior.OpenWhenNoEnemies;

    private Collider2D boxCollider;
    private string gateId;
    private Animator animator;

    [SerializeField] private TriggerEvent TriggerEvent;
    [SerializeField] private TriggerEvent CloseEvent;
    [SerializeField] float timeToRefresh = 1f;

    public DoorBar doorBar;

    private void OnEnable()
    {
        if (behavior == GateBehavior.OpenWhenNoEnemies)
        {
            GameController.Instance.OnEnemyKilled += CheckIfNoEnemies;
        }
        if (TriggerEvent)
        {
            TriggerEvent.OnEventRaised.AddListener(Open);
        }
        if (CloseEvent)
        {
            CloseEvent.OnEventRaised.AddListener(Close);
        }
    }
    private void OnDisable()
    {
        if (behavior == GateBehavior.OpenWhenNoEnemies)
        {
            GameController.Instance.OnEnemyKilled -= CheckIfNoEnemies;
        }
        if (TriggerEvent)
        {
            TriggerEvent.OnEventRaised.RemoveListener(Open);
        }
        if (CloseEvent)
        {
            CloseEvent.OnEventRaised.RemoveListener(Close);
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
        if (animator && boxCollider.enabled) animator.SetTrigger("Open");
        boxCollider.enabled = false;
        if (behavior == GateBehavior.CloseOnTimer)
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndClose());
        }
        
    }

    public void Close()
    {
        if (animator && !boxCollider.enabled) animator.SetTrigger("Close");
        boxCollider.enabled = true;
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

    private IEnumerator WaitAndClose()
    {
        if (doorBar)
        {
            doorBar.StartCountdown(timeToRefresh);
        }
        yield return new WaitForSeconds(timeToRefresh);
        Close();
    }
}
