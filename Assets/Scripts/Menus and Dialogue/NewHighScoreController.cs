using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NewHighScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text textElement;
    private string letterBank = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-_?";
    private char letter1 = 'A';
    private char letter2 = 'A';
    private char letter0 = 'A';

    public bool canEnter = false;
    private int selectedIndex = 0;
    private int selectedLetter = 0;

    private string textTemplate;


    [SerializeField] private float blinkInterval = 0.5f;
    private float blinkTime = 0;
    private bool isBlinking = false;
    // Start is called before the first frame update
    void Start()
    {
        canEnter = false;

        SaveData gameData = SaveDataManager.LoadGameData();
        List<HighScore> highscores = gameData.highScores;


        highscores.Sort((a, b) => b.score.CompareTo(a.score));
        highscores.RemoveAt(highscores.Count - 1);

        HighScore newScore = new HighScore("[PLACEHOLDER]", GameController.Instance.Score);
        highscores.Add(newScore);
        highscores.Sort((a, b) => b.score.CompareTo(a.score));


        textTemplate = "\n";
        int scoreIndex = 0;
        foreach (HighScore highScore in gameData.highScores)
        {
            if (highScore.name == "RABIT")
            {
                textTemplate += $"{highScore.name} ------------   {highScore.score}\n";
            }
            else
            {
                textTemplate += $"{highScore.name}   ------------   {highScore.score}\n";
            }
            scoreIndex++;
        }
        textTemplate += "\n\n------------------------------------------------------\nMessage End";
    }

    // Update is called once per frame
    void Update()
    {
        blinkTime += Time.deltaTime;
        if (blinkTime >= blinkInterval)
        {
            blinkTime = 0;
            isBlinking = !isBlinking;
        }
        UpdateMenuText();
    }

    void OnEnable()
    {
        canEnter = true;
    }

    private void UpdateIndex(int ammount)
    {
        int maxIndex = letterBank.Length - 1;
        // Update Index
        if (selectedIndex + ammount > maxIndex)
        {
            selectedIndex = 0;
        }
        else if (selectedIndex + ammount < 0)
        {
            selectedIndex = maxIndex;
        }
        else
        {
            selectedIndex += ammount;
        }
        // Update LEtter
        if (selectedLetter == 0)
        {
            letter0 = letterBank[selectedIndex];
        }
        else if (selectedLetter == 1)
        {
            letter1 = letterBank[selectedIndex];
        }
        else
        {
            letter2 = letterBank[selectedIndex];
        }

        UpdateMenuText();
    }

    private void UpdateMenuText()
    {
        if (selectedLetter == 0)
        {
            letter0 = letterBank[selectedIndex];
        }
        else if (selectedLetter == 1)
        {
            letter1 = letterBank[selectedIndex];
        }
        else
        {
            letter2 = letterBank[selectedIndex];
        }

        char char0 = (selectedLetter == 0 && isBlinking) ? ' ' : letter0;
        char char1 = (selectedLetter == 1 && isBlinking) ? ' ' : letter1;
        char char2 = (selectedLetter == 2 && isBlinking) ? ' ' : letter2;


        textElement.text = textTemplate.Replace("[PLACEHOLDER]", string.Concat(char0, char1, char2));

    }

    // ==============================================================================
    // Menu Navigation
    // ==============================================================================
    void OnNavigateRight()
    {
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateLeft()
    {
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateDown()
    {
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateUp()
    {
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnSelect()
    {
        if (!canEnter) return;
        if (selectedLetter == 0)
        {
            selectedLetter = 1;
            SetIndexToCorrectCharacter(1);
        }
        else if (selectedLetter == 1)
        {
            selectedLetter = 2;
            SetIndexToCorrectCharacter(2);
        }
        else
        {
            isBlinking = false;
            blinkInterval = 9999f;
            StartCoroutine(WaitAndProceed());
        }
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();


    }

    void OnBack()
    {
        if (selectedLetter == 0)
        {
            // 
        }
        else if (selectedLetter == 1)
        {
            selectedLetter = 0;
            SetIndexToCorrectCharacter(0);
        }
        else
        {
            selectedLetter = 1;
            SetIndexToCorrectCharacter(1);
        }
        if (AudioController.Instance != null) AudioController.Instance.PlayBack();

    }

    private IEnumerator WaitAndProceed()
    {
        GameController.Instance.UpdateHighScores(string.Concat(letter0, letter1, letter2));
        yield return new WaitForSeconds(1f);
        if (GameController.Instance.ShouldSkipGameOver)
        {
            GameController.Instance.SetShouldSkipGameover(false);
            GameController.Instance.ResetGameState();
            GameController.Instance.ChangeScene("Main Menu");

        }
        GameController.Instance.ChangeScene("Game Over Menu");
    }

    private void SetIndexToCorrectCharacter(int characterNumber)
    {
        if (characterNumber == 0)
        {
            selectedIndex = letterBank.IndexOf(letter0);
        }
        else if (characterNumber == 1)
        {
            selectedIndex = letterBank.IndexOf(letter1);

        }
        else
        {
            selectedIndex = letterBank.IndexOf(letter2);

        }
    }
}
