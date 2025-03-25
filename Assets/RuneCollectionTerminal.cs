using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class RuneCollectionTerminal : MonoBehaviour
{
    private GameObject player;
    public GameObject runeObject;

    [SerializeField] private WorldDialogue dialogue;
    private bool wasUsed = false;

    private string id;
    // Start is called before the first frame update
    void Start()
    {
        id = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id)) wasUsed = true;
        player = GameObject.FindGameObjectWithTag("Player");
        if (!wasUsed) runeObject.SetActive(false);

        GameController.Instance.SavedSprites.TryGetValue(id, out Sprite savedSprite);
        if (savedSprite != null)
        {
            if (dialogue != null) dialogue.textToType = "ATTUNED";
            SpriteRenderer screenSr = runeObject.GetComponent<SpriteRenderer>();
            if (screenSr != null)
            {
                screenSr.sprite = savedSprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (!wasUsed && hitCollider.gameObject == player && GameController.Instance.FollowingObjects.Count > 0)
        {

            KeyValuePair<string, FollowingObject> entry = GameController.Instance.FollowingObjects.Last();
            OldFollowPlayer followPlayer = entry.Value.Object.GetComponent<OldFollowPlayer>();
            if (followPlayer)
            {
                followPlayer.CollectForPoints(entry.Key);
                wasUsed = true;
                runeObject.SetActive(true);
                Transform runeTransform = entry.Value.Object.transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("Rune"));
                if (runeTransform)
                {
                    SpriteRenderer runeSr = runeTransform.GetComponent<SpriteRenderer>();
                    SpriteRenderer screenSr = runeObject.GetComponent<SpriteRenderer>();
                    if (runeSr != null && screenSr != null)
                    {
                        screenSr.sprite = runeSr.sprite;
                        GameController.Instance.AddSavedSprite(id, runeSr.sprite);
                        
                        if (dialogue != null) {

                            
                            dialogue.SetTextAs("ATUNEMENT UP!");

                        }
                    }

                }
                GameController.Instance.TagObjectStringAsCollected(id);
            }
            GameController.Instance.TagObjectStringAsCollected(entry.Key);
            GameController.Instance.RemoveFollowingObject(entry.Key);
        }
    }
}
