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
        if (AudioController.Instance != null) AudioController.Instance.PlayButton();
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

            int foundChips = GameController.Instance.SectionChips;
            GameController.Instance.ChangeHealth(foundChips + 1);
            if (foundChips >= 3)
            {
                GameController.Instance.ChangeHealth(1);
            }

            GameController.Instance.StopTimer();
            GameController.Instance.UpdateInstability(1);
            GameController.Instance.UpdateSectionChips(-10);

            Destroy(gameObject);
            GameController.Instance.TagObjectStringAsCollected(id);
            GameController.Instance.TagObjectStringAsCollectedForSession(stringForFunction);
            SaveDataManager.AddPermanentCollectedString(stringForFunction);
            if (AudioController.Instance != null) AudioController.Instance.PlayVictory();


            SaveData saveData = SaveDataManager.LoadGameData();

            // HIGH SCORES
            BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == stringForFunction);
            string highScoreText = "CONGRATULATIONS! NEW HIGHEST YIELD FOR THIS SECTOR";
            if (existingScore != null)
            {
                if (levelScore > existingScore.score)
                {
                    existingScore.score = levelScore;
                    SaveDataManager.SaveGameData(saveData);
                }
                else
                {
                    highScoreText = "Highest VNT Yield For Sector: " + existingScore.score;
                }
            }
            else
            {
                saveData.bestScores.Add(new BestScore(stringForFunction, levelScore));
                SaveDataManager.SaveGameData(saveData);
            }

            // BEST CHIPS
            BestChips chips = saveData.bestChips.FirstOrDefault(bestChip => bestChip.level == stringForFunction);
            if (chips != null)
            {
                if (foundChips > chips.chips)
                {
                    chips.chips = Mathf.Min(foundChips, 3);
                    SaveDataManager.SaveGameData(saveData);
                }
            }
            else
            {
                saveData.bestChips.Add(new BestChips(stringForFunction, Mathf.Min(foundChips, 3)));
                SaveDataManager.SaveGameData(saveData);
            }

            // // BEST TIMES
            // BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == stringForFunction);
            // string bestTimeText = "CONGRATULATIONS! NEW BEST TIME FOR THIS SECTOR";
            // if (existingTime != null)
            // {
            //     if (levelTime < existingTime.time)
            //     {
            //         existingTime.time = levelTime;
            //         SaveDataManager.SaveGameData(saveData);
            //     }
            //     else
            //     {
            //         bestTimeText = "Best Time For Sector: " + ConvertFloatToTime(existingTime.time);
            //     }
            // }
            // else
            // {
            //     saveData.bestTimes.Add(new BestTime(stringForFunction, levelTime));
            //     SaveDataManager.SaveGameData(saveData);
            // }

            if (worldDialogue)
            {
                float multiplier = GlobalConstants.prestigeMultiplier[GameController.Instance.SessionPrestige];
                worldDialogue.textElement.text = "";
                string level = GlobalConstants.checkpointToSector.ContainsKey(GameController.Instance.Checkpoint) ? GlobalConstants.checkpointToSector[GameController.Instance.Checkpoint] : "???";
                string newText = $@" Success! - Message Start
---------------------------------
{level} COMPLETE
VNTs Collected: {pointsBeforeBonus}
CAROTs collected: {foundChips}
Total VNTs: {points}
{highScoreText}

{GetRabitText(foundChips >= 3)}

---------------------------------
Message End";

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
        int totalSeconds = (int)currentTimer;
        float minutes = Mathf.Floor(totalSeconds / 60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }

    private string GetRabitText(bool wasWellDone)
    {
        if (GameController.Instance.Health < 3)
        {
            string[] options = {
            "> 01110100 01101001 01110010 01100100",
            "> 01100010 01100001 01100100",
            "> 01101000 01100001 01110010 01100100",
            "> 01110101 01100111 01101000",
            };
            return options[Random.Range(0, options.Length)];
        }
        else if (wasWellDone)
        {
            string[] options = {
            "> 01000101 01100001 01110011 01111001",
            "> 01100100 01101111 01101110 01100101",
            "> 01111001 01100001 01111001",
            "> 01100101 01111010",
            "> 01111001 01100001 01110100 01100001",
            };
            return options[Random.Range(0, options.Length)];
        }
        else
        {
            string[] options = {
            "> 01100100 01101111 01110101 00111111",
            "> 01101001 01101001 01101011 01100001",
            };
            return options[Random.Range(0, options.Length)];
        }




    }
}
