using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
public class StartingText : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivateUponCompletion;

    [SerializeField] public float timeBetweenCharacters = 0.05f;

    [SerializeField] private TMP_Text textElement;

    private string startingText;
    public bool isFinishedTyping = false;
    private List<HighScore> highScores;

    // Start is called before the first frame update
    void Awake()
    {

        textElement = GetComponent<TMP_Text>();
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        highScores = gameData.highScores;
        // startingText = textElement.text;

        UpdateText();

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

    public void UpdateText(bool finish = false)
    {


        int maxLives = GameController.Instance.MaxHealth;
        int prestige = GameController.Instance.SessionPrestige;
        float healing = GameController.Instance.SessionHealing;
        float multiplier = GameController.Instance.SessionMultiplier;

        float prestigeMultiplier = GlobalConstants.prestigeMultiplier.ContainsKey(prestige) ? GlobalConstants.prestigeMultiplier[prestige] : 1f;
        float lifeMultiplier = GlobalConstants.lifeMultiplier.ContainsKey(maxLives) ? GlobalConstants.lifeMultiplier[maxLives] : 1f;
        float healingMultiplier = GlobalConstants.healingMultiplier.ContainsKey(healing) ? GlobalConstants.healingMultiplier[healing] : 1f;
        // print(SessionPrestige);
        // maxHealth = savedMaxLives;


        startingText = $@"RABIT INSTANTIATED - Readout Start
-----------------------------------

Reset Time : {ParsePrestige(prestige)} - x {prestigeMultiplier.ToString("F2")}
EGO Str    : {maxLives}    - x {lifeMultiplier.ToString("F2")}
EGO Heal   : {ParseHealing(healing)} - x {healingMultiplier.ToString("F2")}
Total      :      [ x {multiplier.ToString("F2")} ]

PREVIOUS RECORDS:

";
        int scoreIndex = 0;
        foreach (HighScore highScore in highScores)
        {
            startingText += $"{highScore.name}   :   {highScore.score}\n";
            scoreIndex++;
        }
        startingText += $@"
LIFT READY,
AWAITING USER INPUT...

-----------------------------------
Readout End";

        if (finish)
        {
            textElement.text = startingText;
        }
    }

    private string ParsePrestige(float prestige)
    {
        switch (prestige)
        {
            case 0:
                return "None";
            case 1:
                return "5:00";
            case 2:
                return "3:00";
            case 3:
                return "1:00";
            default:
            return "Error";
        }
    }

    private string ParseHealing(float healing)
    {
        switch (healing)
        {
            case 50:
                return "5.0%";
            case 25:
                return "2.5%";
            case 10:
                return "1.0%";
            case 0:
                return "None";
            default:
            return "Error";
        }
    }


}


