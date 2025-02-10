using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Button : MonoBehaviour
{
    public enum ButtonAction
    {
        None,
        ActivateEvent,
        ExchangeChipsForPoints,
        CompleteLevel,
    }
    [SerializeField] private ButtonAction action = ButtonAction.None;

    [SerializeField] private bool requiresFirmPress = false;

    [SerializeField] private bool oneTimeUse = true;

    private Collider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject playerAttack;

    [SerializeField] private string stringForFunction;

    [SerializeField] private TriggerEvent eventToTrigger;
    [SerializeField] private WorldDialogue worldDialogue;

    private string id;

    [SerializeField] float timeToRefresh = 1f;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

            if (playerBottom > buttonTop && (!requiresFirmPress || hitPlayer.IsDashing()))
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
                GameController.Instance.TagObjectStringAsCollected(entry.Value.Name);

                idsToRemove.Add(entry.Key);

            }
            int multiplier = 1;
            int totalPointsAdded = 0;


            foreach (string key in idsToRemove)
            {
                totalPointsAdded += (5000 * multiplier);

                GameController.Instance.TagObjectStringAsCollected(key);

                GameController.Instance.AddToScore(5000 * multiplier);
                GameController.Instance.RemoveFollowingObject(key);
                multiplier++;
            }

            Destroy(gameObject);
            if (oneTimeUse) GameController.Instance.TagObjectStringAsCollected(id);
            if (worldDialogue)
            {
                worldDialogue.textElement.text = "";
                string newText = $@"> CHIPS DEPOSITED: {multiplier - 1}
> POINTS ADDED: {totalPointsAdded}";

                worldDialogue.startingText = newText;
                worldDialogue.textToType = newText;
            }

            if (stringForFunction != null && stringForFunction.Length > 0)
            {
                SaveDataManager.AddPermanentCollectedString(stringForFunction);
            }

            if (eventToTrigger != null)
            {
                eventToTrigger.Raise();
            }
        }

        if (action == ButtonAction.ActivateEvent)
        {
            if (oneTimeUse)
            {
                Destroy(gameObject);
                GameController.Instance.TagObjectStringAsCollected(id);
            }
            else
            {
                if (spriteRenderer && boxCollider)
                {
                    spriteRenderer.enabled = false;
                    boxCollider.enabled = false;
                    StartCoroutine(WaitAndReEnableButton());
                }

            }
            if (eventToTrigger != null)
            {
                eventToTrigger.Raise();
            }

        }

        if (action == ButtonAction.CompleteLevel)
        {
            if (eventToTrigger != null)
            {
                eventToTrigger.Raise();
            }
            float remainingTime = GameController.Instance.BonusTimer;
            int pointsBeforeBonus = GameController.Instance.Score;
            

            int points = GameController.Instance.Score;
            
            int levelScore = points - GameController.Instance.StartingScore;
            float levelTime = GameController.Instance.StartingTimer - remainingTime;
            
            GameController.Instance.StopTimer();

            Destroy(gameObject);
            GameController.Instance.TagObjectStringAsCollected(id);
            SaveDataManager.AddPermanentCollectedString(stringForFunction);


            SaveData saveData = SaveDataManager.LoadGameData();
            BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == stringForFunction);
            string highScoreText = "CONGRATULATIONS! NEW HIGH SCORE FOR THIS SECTOR";
            if (existingScore != null)
            {
                if(levelScore > existingScore.score)
                {
                existingScore.score = levelScore;
                SaveDataManager.SaveGameData(saveData);    
                }
                else 
                {
                    highScoreText = "Highest Score For Sector: "+existingScore.score;
                }
            }
            else
            {
                saveData.bestScores.Add(new BestScore(stringForFunction, levelScore));
                SaveDataManager.SaveGameData(saveData);    
            }

            BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == stringForFunction);
            string bestTimeText = "CONGRATULATIONS! NEW BEST TIME FOR THIS SECTOR";
            if (existingTime != null)
            {
                if(levelTime < existingTime.time)
                {
                existingTime.time = levelTime;
                SaveDataManager.SaveGameData(saveData);    
                }
                else 
                {
                    bestTimeText = "Best Time For Sector: " + ConvertFloatToTime(existingTime.time);
                }
            }
            else
            {
                saveData.bestTimes.Add(new BestTime(stringForFunction, levelTime));
                SaveDataManager.SaveGameData(saveData);    
            }

            if (worldDialogue)
            {
                float multiplier = GlobalConstants.prestigeMultiplier[GameController.Instance.SessionPrestige];
                worldDialogue.textElement.text = "";
                string newText = $@"> {GameController.Instance.Checkpoint} COMPLETE
> Cognition Data Collected: {pointsBeforeBonus}
> Total Data Collected: {points}
> {highScoreText}
> {bestTimeText}";

                worldDialogue.startingText = newText;
                worldDialogue.textToType = newText;
            }

            GameController.Instance.SetCheckPoint("Main Menu");
        }

    }
    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator WaitAndReEnableButton()
    {
        yield return new WaitForSeconds(timeToRefresh);
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;

    }

    private string ConvertFloatToTime(float currentTimer)
    {
        if (currentTimer <= 0) return "CRITICAL";
        int totalSeconds = (int) currentTimer;
        float minutes = Mathf.Floor(totalSeconds/60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }
}
