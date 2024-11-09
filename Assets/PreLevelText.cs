using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLevelText : MonoBehaviour
{
    DialogueManager dialogueManager;
    [SerializeField] private string previousLevel = "";
    // Start is called before the first frame update
    void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
        Dialogue dialogueCopy = dialogueManager.currentDialogue.Clone();
        

        // Score
        if (previousLevel.Length > 0 && GameController.Instance.Score > 0)
        {
            DialogueSentence sentence = new DialogueSentence();
            sentence.text = $@"> Congratulations!
> {previousLevel} Completed!
> Score: {GameController.Instance.Score}
";
            sentence.dialogueOptions = new List<DialogueOption>();
            dialogueCopy.dialogueSentences.Insert(0,sentence);
        }

        // Remaining Time
        DialogueSentence time = new DialogueSentence();
            time.text = $@"> -CRITICAL SYSTEM ERROR DETECTED-
> System will restart in {ConvertFloatToTime(GameController.Instance.BonusTimer)}
> Reset will maintain overall system integrity.
";
        time.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(time);

        dialogueManager.currentDialogue = dialogueCopy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string ConvertFloatToTime(float currentTimer)
    {
        int timer = (int) currentTimer;
        float minutes = Mathf.Floor(timer/60);
        int seconds = timer % 60;
        return minutes + ":" + seconds.ToString("D2");
    }
}
