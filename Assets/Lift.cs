using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lift : MonoBehaviour
{
    private string id;
    private bool isActive = false;
    [SerializeField] private string requiredKey;
    [SerializeField] private DialogueEvent ActivateEvent;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private string targetSceneName;
    [SerializeField] private Material outlineMaterial; private SpriteRenderer spriteRenderer;
    private Material startingMaterial;
    private Transform playerTransform;

    [SerializeField] private WaypointMovement waypointMovement;




    void Start()
    {
        id = $"Lift{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id)) isActive = true;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingMaterial = spriteRenderer.material;

    }

    // Update is called once per frame
    void Update()
    {
        if (CanInteractWith() && outlineMaterial)
        {
            spriteRenderer.material = outlineMaterial;
        }
        else
        {
            spriteRenderer.material = startingMaterial;
        }
    }

    private void OnEnable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (ActivateEvent)
        {
            ActivateEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    private bool CanInteractWith()
    {
        if (!playerTransform || !interactionPoint || !isActive) return false;
        if (Vector2.Distance(interactionPoint.position, playerTransform.position) <= interactionDistance) return true;
        return false;
    }

    void OnInteract()
    {
        if (CanInteractWith())
        {
            
            if (waypointMovement)
            {
                waypointMovement.TriggerShouldMove();
                Player playerScript = playerTransform.GetComponent<Player>();
                // LevelConnection.ActiveConnection = levelConnection;
                if (playerScript) playerScript.RemovePlayer();
                GameController.Instance.ChangeScene(targetSceneName);
            }
        }
    }

    void OnEventRaised()
    {
        isActive = true;
        GameController.Instance.TagObjectStringAsCollected(id);
        GameController.Instance.RemoveFollowingObject(requiredKey);
    }
}
