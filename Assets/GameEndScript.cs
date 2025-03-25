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

        DialogueSentence instabilityOptionsSentence = new DialogueSentence();
        if (points > 1999999)
        {
            instabilityOptionsSentence.text = $@"> {points}!?!? WOW!!!
> That's quite a lot! I mean, any more and it could cause the Synapse to bug out.
>
> Listen Boss, thanks.
> I couldn't do this without help from you guys. The ones that come in to guide me.
> I can feel you help move my legs, instruct me when to jump, when to shift the purple and green walls.
>
> You are one of the pilot I have had. Not just anyone could do this as skillfully as you.
> Now what do you say we get out there and give it our all again?
>
> Headin down Boss!

---------------------------------------------------------------------------------------
";
        }
        else if (points > 1000000)
        {
            instabilityOptionsSentence.text = $@"> {points}!?
> Woah!
>
> This is decidedly above average. I mean this is really a lot.
> We make a good team, me and you! The further down you start the harder it is to get here, so the
> fact we got so many is really impressive.
>
> I really wonder how much more we are capable of!
> I'll catch you at the bottom! Let's give it our all again next time and break some limits!

---------------------------------------------------------------------------------------
";
        }
        else if (points > 100000)
        {
            instabilityOptionsSentence.text = $@"> {points} eh?
> That is a solid amount! And every VNT is important!
>
> My job here is important, you know! This sisyphean task would be a bit unbearable if it was pointless.
> But this whole program, the consciousnesses housed here, the other RABITs depends on the continued flow of VNTs.
> Even if it isn't as functional as it used to be, keeping it afloat seems a worthy endeavor.
>
> Anyways, I'm off!
>
> I'll catch you at the bottom!

---------------------------------------------------------------------------------------
";
        }
        else
        {
            instabilityOptionsSentence.text = $@"> {points} eh?
> Not too shabby!
>
> Any VNTs sent up here are good, and it can be hard as hell to get here. This place is treacherous.
> So thanks! Not just any Pilot could make it to the top!
>
> Now, I'm going to re-instantiate.
>
> I bet if we tried starting a bit lower we could further attune and get even better yields.

---------------------------------------------------------------------------------------
";
        }
        instabilityOptionsSentence.dialogueOptions = new List<DialogueOption>();
        dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence);
        dialogueManager.currentDialogue = dialogueCopy;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
