using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelStart : MonoBehaviour
{
    [SerializeField] private float levelTime = 60f;
    [SerializeField] private WorldDialogue worldDialogue;

    private Player player;
    private Vector2 playerPosition;
    private string currentLevel;


    private bool waitingToStartLevel = false;

    // Start is called before the first frame update
//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
//         playerPosition = player.transform.position;
//         currentLevel = SceneManager.GetActiveScene().name;

//         if (GameController.Instance.Checkpoint != currentLevel)
//         {

//             if (GlobalConstants.timers.ContainsKey(currentLevel) && GlobalConstants.timers[currentLevel].ContainsKey(GameController.Instance.SessionPrestige))
//             {
//                 float timeSet = GlobalConstants.timers[currentLevel][GameController.Instance.SessionPrestige];
//                 GameController.Instance.StartTimer(timeSet);
//                 GameController.Instance.SetStartingTimer(timeSet);

//             }
//             else
//             {
//                 GameController.Instance.StartTimer(levelTime);
//                 GameController.Instance.SetStartingTimer(levelTime);
//             }

//             GameController.Instance.SetCheckPoint(currentLevel);
//             GameController.Instance.SetStartingScore(GameController.Instance.Score);

//             if (worldDialogue)
//             {

//                 SaveData saveData = SaveDataManager.LoadGameData();
//                 BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == GameController.Instance.Checkpoint);
//                 string highScoreText = "Sector Has Yet To Be Cleared";
//                 if (existingScore != null)
//                 {
//                     highScoreText = "Highest Score For Sector: " + existingScore.score;
//                 }


//                 worldDialogue.textElement.text = "";
//                 string newText = $@"> {currentLevel} Loaded To Memory
// > {highScoreText}";

//                 worldDialogue.startingText = newText;
//                 worldDialogue.textToType = newText;
//                 worldDialogue.Activate();
//             }
//         }


//     }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        playerPosition = player.transform.position;
        currentLevel = SceneManager.GetActiveScene().name;

        if (GameController.Instance.Checkpoint != currentLevel)
        {
            if (GlobalConstants.prestigeTime.ContainsKey(GameController.Instance.SessionPrestige))
            {
                float timeSet = GlobalConstants.prestigeTime[GameController.Instance.SessionPrestige];
            
                GameController.Instance.StartTimer(timeSet);
                GameController.Instance.SetStartingTimer(timeSet);

            }
            else
            {
                GameController.Instance.StartTimer(levelTime);
                GameController.Instance.SetStartingTimer(levelTime);
            }

            GameController.Instance.SetCheckPoint(currentLevel);
            GameController.Instance.SetStartingScore(GameController.Instance.Score);

            if (worldDialogue)
            {

                SaveData saveData = SaveDataManager.LoadGameData();
                BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == GameController.Instance.Checkpoint);
                string highScoreText = "Sector Has Yet To Be Cleared";
                if (existingScore != null)
                {
                    highScoreText = "Highest Score For Sector: " + existingScore.score;
                }


                worldDialogue.textElement.text = "";
                string newText = $@"> {currentLevel} Loaded To Memory
> {highScoreText}";

                worldDialogue.startingText = newText;
                worldDialogue.textToType = newText;
                worldDialogue.Activate();
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        //if this level has just been started
        // if (waitingToStartLevel && (Mathf.Abs(player.transform.position.x - playerPosition.x) > 0.2f || Mathf.Abs(player.transform.position.y - playerPosition.y) > 0.2f))
        // {
        //     GameController.Instance.StartTimer(levelTime);
        //     waitingToStartLevel = false;
        // }
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
}
