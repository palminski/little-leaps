using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lift : MonoBehaviour
{
    private string id;
    private Animator animator;
    [SerializeField] private bool isActive = false;
    [SerializeField] private string requiredKey;
    [SerializeField] private DialogueEvent ActivateEvent;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private string targetSceneName;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private SpriteRenderer veins;
    private SpriteRenderer spriteRenderer;
    private Material startingMaterial;
    private Transform playerTransform;
    private Player playerScript;

    public bool isMoving = false;

    [SerializeField] private WaypointMovement waypointMovement;




    void Start()
    {
        id = $"Lift{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id)) isActive = true;
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = playerTransform.GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingMaterial = spriteRenderer.material;

    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor = veins.color;
        Color.RGBToHSV(currentColor, out float h, out float s, out float v);
        v = isActive ? 1 : 0.25f;
        Color newColor = Color.HSVToRGB(h, s, v);
        veins.color = newColor;
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

    

    void OnEventRaised()
    {
        Dictionary<string, FollowingObject> followingObjects = GameController.Instance.FollowingObjects;
        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value.Name == requiredKey)
            {
                isActive = true;
                GameController.Instance.TagObjectStringAsCollected(entry.Key);
                GameController.Instance.TagObjectStringAsCollected(id);
                GameController.Instance.RemoveFollowingObject(entry.Key);
                break;
            }
        }
        
    }

    void OnTriggerStay2D(Collider2D hitCollider)
    {
        if (!isMoving && isActive && hitCollider.gameObject == playerTransform.gameObject && playerScript.IsGrounded()) {
                if (playerScript) playerScript.SetInputEnabled(false);
                spriteRenderer.sortingOrder = 60;
                animator.SetTrigger("Close Lift");
                // GameController.Instance.ChangeScene(targetSceneName);
                isMoving = true;
                StartCoroutine(WaitAndChangeScene());
        }
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(1.4f);
    
        if (playerScript) playerScript.RemovePlayer();
        waypointMovement.TriggerShouldMove();
        yield return new WaitForSeconds(0.5f);
        GameController.Instance.ChangeScene(targetSceneName);
    }
}


