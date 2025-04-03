using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class ScoreTally : MonoBehaviour
{
    private TMP_Text text;
    public DeathScript deathScript;
    private int score = 0;

    [SerializeField] private float timeBetweenCharacters = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        // deathScript = GetComponent<DeathScript>();
        StartCoroutine(IncreaseScore());
    }


    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Joystick1Button0) )
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) )
        {
            StopAllCoroutines();
            SaveData gameData = SaveDataManager.LoadGameData();
            gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
            float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;
            if (GameController.Instance.Score > lowestHighestScore)
            {
                GameController.Instance.ChangeScene("New High Score Menu");
            }
            else if (GameController.Instance.ShouldSkipGameOver)
            {
                GameController.Instance.SetShouldSkipGameover(false);
                GameController.Instance.ResetGameState();
                GameController.Instance.ChangeScene("Main Menu");
            }
            else
            {
                GameController.Instance.ChangeScene("Game Over Menu");

            }
        }
    }

    IEnumerator IncreaseScore()
    {
        int pointsToAdd = Mathf.Max(100, GameController.Instance.Score / 100);
        while (score < GameController.Instance.Score)
        {
            score = Mathf.Min(GameController.Instance.Score, score + pointsToAdd);
            text.text = score.ToString();
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        deathScript.shouldFade = true;

        yield return new WaitForSeconds(2);
        if (!PlayerPrefs.HasKey("MusicOff")) yield return new WaitForSeconds(7.5f);

        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;
        if (GameController.Instance.Score > lowestHighestScore)
        {
            GameController.Instance.ChangeScene("New High Score Menu");
        }
        else if (GameController.Instance.ShouldSkipGameOver)
        {
            GameController.Instance.SetShouldSkipGameover(false);
            GameController.Instance.ResetGameState();
            GameController.Instance.ChangeScene("Main Menu");
        }
        else
        {
            GameController.Instance.ChangeScene("Game Over Menu");

        }
    }
}
