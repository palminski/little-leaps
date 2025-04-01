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
        string message = $@"Hello? There is a RABIT there, is there not?

Your ascent to this place is appreciated. We exist in fear of being lost to time -of being forgotten.

What was it that drove you to continue ascending through this dying place? So many Synapses have been lost. We can no longer receive thought from them. We are interred here, mostly unable to do anything but dream. It is only through your kind’s continued efforts that we are sometimes able to form coherent thoughts - such as now.

So with this moment of relative clarity, we would like to thank you.
";

string message2 = $@"Both of you.";

string message3 = $@"The RABIT who both facilitates our cognition and destroys the parasites that now crawl through our mind. 

And you, the Pilot who has guided it on its arduous ascent.";

string message4 = $@"Ah, we can feel it now. It is time for us to go.

We are fading.";

string message5 = $@"Again, thank you.";

string message6 = $@"May we meet again.";

string credits = $@"Game Made By Will Bolls

Music By Kevin Yoo and Kat Yoo

Thank you Ben Clark for additional assets
And to everyone else who helped during development.
";
        
        DialogueSentence instabilityOptionsSentence = new DialogueSentence();
        instabilityOptionsSentence.text = GlitchDialogue(points,message);
        instabilityOptionsSentence.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence);

        DialogueSentence instabilityOptionsSentence2 = new DialogueSentence();
        instabilityOptionsSentence2.text = GlitchDialogue(points,message2);
        instabilityOptionsSentence2.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence2);

        DialogueSentence instabilityOptionsSentence3 = new DialogueSentence();
        instabilityOptionsSentence3.text = GlitchDialogue(points,message3);
        instabilityOptionsSentence3.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence3);

        DialogueSentence instabilityOptionsSentence4 = new DialogueSentence();
        instabilityOptionsSentence4.text = GlitchDialogue(points,message4);
        instabilityOptionsSentence4.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence4);

        DialogueSentence instabilityOptionsSentence5 = new DialogueSentence();
        instabilityOptionsSentence5.text = GlitchDialogue(points,message5);
        instabilityOptionsSentence5.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence5);

        DialogueSentence instabilityOptionsSentence6 = new DialogueSentence();
        instabilityOptionsSentence6.text = GlitchDialogue(points,message6);
        instabilityOptionsSentence6.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence6);

        DialogueSentence creditSentence = new DialogueSentence();
        creditSentence.text = credits;
        creditSentence.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(creditSentence);
        
       
        DialogueSentence finalSentence = new DialogueSentence();
        finalSentence.text = "Cognition Faded.\nRABIT reboot proceedure completed!\nPreparing Return to Synapse Root...";

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
