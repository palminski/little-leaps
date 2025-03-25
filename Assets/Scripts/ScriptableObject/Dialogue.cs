using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;

[Serializable]
public class DialogueSentence
{
    [TextArea(3,10)]
    public string text;
    public List<DialogueOption> dialogueOptions;

    public DialogueSentence Clone()
    {
        DialogueSentence newSentence = new DialogueSentence();

        newSentence.text = this.text;
        newSentence.dialogueOptions = new List<DialogueOption>();

        foreach (DialogueOption option in dialogueOptions)
        {
            newSentence.dialogueOptions.Add(option.Clone());
        }

        return newSentence;
    }
}

[Serializable]
public class DialogueOption
{
    public string option;
    public Dialogue nextDialogue;
    public DialogueEvent optionEvent;

    public TriggerEvent triggerEvent;
    [TextArea(15, 15)] public string sideText;

    public DialogueOption Clone()
    {
        DialogueOption newOption = new DialogueOption();
        newOption.option = this.option;
        newOption.nextDialogue = this.nextDialogue != null ? this.nextDialogue.Clone() : null;
        newOption.optionEvent = this.optionEvent;
        newOption.sideText = this.sideText;
        newOption.triggerEvent = this.triggerEvent;
        return newOption;
    }
}

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueSentence> dialogueSentences;

    public Dialogue Clone()
    {
        Dialogue newDialogue = ScriptableObject.CreateInstance<Dialogue>();
        newDialogue.dialogueSentences = new List<DialogueSentence>();

        foreach (DialogueSentence sentence in dialogueSentences)
        {
            newDialogue.dialogueSentences.Add(sentence.Clone());
        }
        return newDialogue;
    }
}
