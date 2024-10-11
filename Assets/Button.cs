using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public enum ButtonAction
    {
        None,
        ActivateEvent,
        ExchangeChipsForPoints,
    }
    [SerializeField] private ButtonAction action = ButtonAction.None;

    [SerializeField] private bool requiresFirmPress = false;
    private Collider2D boxCollider;
    private GameObject playerAttack;

    [SerializeField] private TriggerEvent eventToTrigger;
    [SerializeField] private WorldDialogue worldDialogue;

    private string id;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        playerAttack = GameObject.FindGameObjectWithTag("PlayerAttack");
        id = $"BUTTON-{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {


        // - Check if player can hurt enemy
        if (hitCollider.gameObject == playerAttack)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Player hitPlayer = player.GetComponent<Player>();

            float playerBottom = hitPlayer.GetLastPosition().y + hitCollider.offset.y - (hitCollider.bounds.size.y / 2);
            float buttonTop = transform.position.y + boxCollider.offset.y + boxCollider.bounds.size.y / 2;

            if (playerBottom > buttonTop && hitPlayer.IsDashing())
            {
                PerformButtonAction();
            }

        }
    }

    private void PerformButtonAction()
    {

        if (action == ButtonAction.ExchangeChipsForPoints)
        {
            
            List<string> idsToRemove = new List<string>();
            Dictionary<string, FollowingObject> followingObjects = GameController.Instance.FollowingObjects;
            foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
            {
                if (entry.Value.Name == "chip")
                {
                    idsToRemove.Add(entry.Key);
                }
            }
            int multiplier = 1;
            int totalPointsAdded = 0;
            int totalTimeAdded = 0;
            foreach (string key in idsToRemove)
            {
                totalPointsAdded += (5000 * multiplier);
                totalTimeAdded += 30;
                GameController.Instance.TagObjectStringAsCollected(key);
                GameController.Instance.AddToTimer(30);
                GameController.Instance.AddToScore(5000 * multiplier);
                GameController.Instance.RemoveFollowingObject(key);
                multiplier++;

                
            }
            Destroy(gameObject);
            GameController.Instance.TagObjectStringAsCollected(id);
            if (worldDialogue)
            {
                worldDialogue.textElement.text = "";
                string newText = $@"> SUCCESS! TIMER EXTENDED
> CHIPS DEPOSITED: {multiplier - 1}
> POINTS ADDED: {totalPointsAdded}
> TIME ADDED TO RESET TIME: {totalTimeAdded}";

                worldDialogue.startingText = newText;
                worldDialogue.textToType = newText;
            }

            if (eventToTrigger != null)
            {
                eventToTrigger.Raise();
            }
        }
        if (action == ButtonAction.ActivateEvent)
        {
            Destroy(gameObject);
            GameController.Instance.TagObjectStringAsCollected(id);
            if (eventToTrigger != null)
            {
                eventToTrigger.Raise();
            }
        }


    }
    // Update is called once per frame
    void Update()
    {

    }
}
