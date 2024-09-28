using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class StartingText : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivateUponCompletion;

    [SerializeField] public float timeBetweenCharacters = 0.05f;

    [SerializeField] private TMP_Text textElement;

    private string startingText;
    public bool isFinishedTyping = false;
    private int currentIndex;
    // Start is called before the first frame update
    void Awake()
    {

        textElement = GetComponent<TMP_Text>();
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a,b) => b.CompareTo(a));
        // startingText = textElement.text;
        startingText = $@"SYSTEM RESET INITIATED...
[SUCCESS] - CURRENT RESET: 776776
PREVIOUS RECORDS:

";
int scoreIndex = 0;
foreach(int score in gameData.highScores)
{
    startingText += $"RECORD_{scoreIndex} = {score}\n";
    scoreIndex++;
}
startingText += $@"
READY TO BEGIN
AWAITING USER INPUT...";
        
        currentIndex = 0;
        StartCoroutine(TypeSentence(startingText));

    }

    // Update is called once per frame
    void Update()
    {

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
        isFinishedTyping = true;
        Destroy(GetComponent<PlayerInput>());
    }

    public void SetSideText(string text)
    {
        textElement.text = (text.Length > 0) ? text : startingText;
    }
}


