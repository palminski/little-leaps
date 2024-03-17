using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class DialogueSentence
{
    [TextArea(3,10)]
    public string text;
    public List<DialogueOption> dialogueOptions;
}

[Serializable]
public class DialogueOption
{
    public string option;
    public Dialogue nextDialogue;
    public DialogueEvent optionEvent;
}

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueSentence> dialogueSentences;
}
