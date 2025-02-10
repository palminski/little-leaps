using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLevelText : MonoBehaviour
{
    DialogueManager dialogueManager;
    [SerializeField] private string previousLevel = "";
    [SerializeField] private string targetLevel = "";
    [SerializeField] private string targetScene = "";
    // Start is called before the first frame update
    void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
        Dialogue dialogueCopy = dialogueManager.currentDialogue.Clone();
        string startingText = dialogueManager.textElement.text;
        

        //         > [PREVIOUSLEVEL] Complete
        // > Current Score: [SCORE]


        // Score
        if (previousLevel.Length > 0 && GameController.Instance.Score > 0)
        {
            string previousLevelText = $@"
> {previousLevel} Complete
> Current Cognitive Data: {GameController.Instance.Score}
";
            startingText = startingText.Replace("[PREVIOUSLEVELTEXT]", previousLevelText);
        }
        else
        {
            startingText = startingText.Replace("[PREVIOUSLEVELTEXT]", "");

        }
        startingText = startingText.Replace("[NEXTLEVEL]", targetLevel);

        startingText = startingText.Replace("[TIMERSTATUS]", GlobalConstants.prestigeText.ContainsKey(GameController.Instance.SessionPrestige) ? GlobalConstants.prestigeText[GameController.Instance.SessionPrestige] : "CRITICAL");
        // if (GlobalConstants.timers.ContainsKey(targetScene) && GlobalConstants.timers[targetScene].ContainsKey(GameController.Instance.SessionPrestige))
        // {
        //     float timeSet = GlobalConstants.timers[targetScene][GameController.Instance.SessionPrestige];
        //     startingText = startingText.Replace("[TIMER]", ConvertFloatToTime(timeSet));
        // }

        DialogueSentence sentence = new DialogueSentence();
        sentence.text = startingText;
        sentence.dialogueOptions = new List<DialogueOption>();

        dialogueCopy.dialogueSentences.Clear();
        dialogueCopy.dialogueSentences.Add(sentence);
        // dialogueCopy.dialogueSentences.Insert(0, sentence);

        dialogueManager.currentDialogue = dialogueCopy;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private string ConvertFloatToTime(float currentTimer)
    {
        if (currentTimer <= 0) return "00:00:00";
        int totalSeconds = (int)currentTimer;
        float minutes = Mathf.Floor(totalSeconds / 60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }
}
