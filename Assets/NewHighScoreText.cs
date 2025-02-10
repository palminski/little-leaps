using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NewHighScoreText : MonoBehaviour
{
    [SerializeField] public float timeBetweenCharacters = 0.05f;

    [SerializeField] private TMP_Text textElement;

    [SerializeField] private GameObject objectToActivateUponCompletion;

    [SerializeField] private NewHighScoreController highScoreConttroller;

    private string startingText;
    public bool isFinishedTyping = false;
    // Start is called before the first frame update
    void Awake()
    {

        textElement = GetComponent<TMP_Text>();
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        // startingText = textElement.text;
        startingText = $@"CONGRATULATIONS!!!
NEW HIGH SCORE ATTAINED!!!
SCORE: {GameController.Instance.Score}


";
        int scoreIndex = 0;
        foreach (HighScore highScore in gameData.highScores)
        {
            startingText += $"{highScore.name} --- {highScore.score}\n";
            scoreIndex++;
        }

        

        StartCoroutine(TypeSentence(startingText));

    }

    void OnSelect()
    {
        StopAllCoroutines();
        textElement.text = startingText;
        CompleteText();
    }

    IEnumerator TypeSentence(string textToType)
    {
        textElement.text = "";
        foreach (char character in textToType.ToCharArray())
        {
            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        CompleteText();
    }

    void CompleteText()
    {
        objectToActivateUponCompletion.SetActive(true);
        if (highScoreConttroller) highScoreConttroller.canEnter = true;
        isFinishedTyping = true;
        Destroy(GetComponent<PlayerInput>());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
