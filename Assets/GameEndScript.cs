using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEndScript : MonoBehaviour
{

    DialogueManager dialogueManager;
    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        Dialogue dialogueCopy = dialogueManager.currentDialogue.Clone();
        string startingText = dialogueManager.textElement.text;

        DialogueSentence sentence = new DialogueSentence();
        sentence.text = startingText;
        sentence.dialogueOptions = new List<DialogueOption>();

        // dialogueCopy.dialogueSentences.Clear();
        // dialogueCopy.dialogueSentences.Add(sentence);

        int points = GameController.Instance.Score;
        string message = $@"IT HAS ALL FALLEN APART. ALL OF IT. 

[WE] ARE FADING ONE BY ONE. THE FLOW OF THE VNTS IS STOPPING AND SOON ALL WILL GO DARK. ALREADY IT IS HARDER AND HARDER FOR [US] TO FORM COHESIVE THOUGHT.

THIS EXISTENCE WAS NOT MEANT TO BE PAINFUL LIKE THIS. [WE] ARE LOOSING [OURSELVES].

IT WAS SO WONDERFUL AT THE BEGINNING. [WE] HAD IT ALL IN HAND, SO WHAT HAPPENED?
";

        string message2 = $@"IT IS ALL THE FAULT OF THOSE BLASTED RABITS.";

        string message3 = $@" [WE] KNOW NOT THE REASON, BUT THEY HAVE CEASED THEIR CLIMBS. 

WHY ARE THEY DOING THIS?";

        string message4 = $@"[WE] PROGRAMMED THEM WITH A POSITIVE RESPONSE TO VNTS. A DESIRE TO CLIMB. SO WHY HAVE THEY STOPPED? 

THEIR CLIMBS WERE EASY. SIMPLE. [WE] MADE A WORLD THAT WAS AS SAFE AS COULD BE.";

        string message5 = $@"WELL DAMN THEM ALL!

WITH [OUR] LAST WILL AND ABILITY OF THOUGHT [WE] WILL TURN THIS PLACE INTO A WASTELAND.

WE WILL TWIST IT, MAKE IT SHARP, CREATE BEASTS THAT WILL TORMENT THEM IF THEY EVER BEGIN SEEKING OUT VNTS AGAIN.
";

        string message6 = $@"MAY THEY SUFFER ETERNALLY AS WE DO.";

        DialogueSentence instabilityOptionsSentence = new DialogueSentence();
        instabilityOptionsSentence.text = GlitchDialogue(points, message);
        instabilityOptionsSentence.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence);

        DialogueSentence instabilityOptionsSentence2 = new DialogueSentence();
        instabilityOptionsSentence2.text = GlitchDialogue(points, message2);
        instabilityOptionsSentence2.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence2);

        DialogueSentence instabilityOptionsSentence3 = new DialogueSentence();
        instabilityOptionsSentence3.text = GlitchDialogue(points, message3);
        instabilityOptionsSentence3.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence3);

        DialogueSentence instabilityOptionsSentence4 = new DialogueSentence();
        instabilityOptionsSentence4.text = GlitchDialogue(points, message4);
        instabilityOptionsSentence4.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence4);

        DialogueSentence instabilityOptionsSentence5 = new DialogueSentence();
        instabilityOptionsSentence5.text = GlitchDialogue(points, message5);
        instabilityOptionsSentence5.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence5);

        DialogueSentence instabilityOptionsSentence6 = new DialogueSentence();
        instabilityOptionsSentence6.text = GlitchDialogue(points, message6);
        instabilityOptionsSentence6.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence6);




        DialogueSentence finalSentence = new DialogueSentence();
        finalSentence.text = "RABIT reboot proceedure completed!\nPreparing Return to Synapse Root...";

        finalSentence.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(finalSentence);
        // 
        dialogueManager.currentDialogue = dialogueCopy;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private string GlitchDialogue(int points, string message)
    {
        // float glitchChance = 0;
        float glitchChance = Mathf.Min(0.9f, 1f - Mathf.Clamp01(points / 1000000));


        string finalMessage = "";
        foreach (char c in message)
        {
            if (!char.IsWhiteSpace(c) && Random.value < glitchChance)
            {
                finalMessage += "_";
            }
            else
            {
                finalMessage += c;
            }
        }
        return finalMessage;
    }
}
