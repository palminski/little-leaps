using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameOverText : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivateUponCompletion;

    [SerializeField] public float timeBetweenCharacters = 0.05f;

    [SerializeField] private TMP_Text textElement;

    private string startingText;
    public bool isFinishedTyping = false;

    // Start is called before the first frame update
    void Awake()
    {

        textElement = GetComponent<TMP_Text>();
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        // startingText = textElement.text;
        startingText = $@"Operator Desync - Message Start
------------------------------------------------------
Operator's EGO presence in RABIT diminished.
Desynchronisation has occurred.
VNTs Registered: {GameController.Instance.Score}

Please Select Option Below:
";


        GameController.Instance.ResetGameState();

        StartCoroutine(TypeSentence(startingText));

    }

    // Update is called once per frame
    void Update()
    {
        if (!isFinishedTyping && Input.anyKeyDown)
        {
            StopAllCoroutines();
            textElement.text = startingText;
            if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
            StartCoroutine(WaitAndCompleteText());
        }
    }

    IEnumerator WaitAndCompleteText()
    {
        yield return new WaitForSeconds(0.05f);
        CompleteText();
    }

    

    IEnumerator TypeSentence(string textToType)
    {
        textElement.text = "";
        foreach (char character in textToType.ToCharArray())
        {
            if (character != ' ') if (AudioController.Instance != null) AudioController.Instance.PlayTypingBeep();
            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        CompleteText();
    }

    void CompleteText()
    {
        objectToActivateUponCompletion.SetActive(true);
        isFinishedTyping = true;
        Destroy(GetComponent<PlayerInput>());
    }

    public void SetSideText(string text)
    {
        textElement.text = (text.Length > 0) ? text : startingText;
    }
}
