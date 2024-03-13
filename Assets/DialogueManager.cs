using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text textElement;
    [SerializeField]
    public TMP_Text optionsElement;

    [SerializeField]
    public float timeBetweenCharacters = 0.05f;

    public Dialogue currentDialogue;

    private int currentIndex;

    private int dialogueOptionIndex;

    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {

        currentIndex = 0;
        StartCoroutine(TypeSentence(currentDialogue.dialogueSentences[currentIndex].text));
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerInput.enabled = false;

    }

    void OnDestroy()
    {
        if (playerInput) playerInput.enabled = true;
    }

    void OnSelect()
    {
        if (IsTyping())
        {
            CompleteSentence();
            return;
        }
        if (AreDialogueOptionsVisable()) 
        {
            DialogueOption dialogueOption = currentDialogue.dialogueSentences[currentIndex].dialogueOptions[dialogueOptionIndex];

            if(dialogueOption.nextDialogue) 
            {
                currentDialogue = dialogueOption.nextDialogue;
                currentIndex = 0;
                StartCoroutine(TypeSentence(currentDialogue.dialogueSentences[currentIndex].text));
                return;
            }
        }
        DisplayNextSentence();
    }

    void OnNavigateLeft() {
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(-1);
    }

    void OnNavigateRight() {
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(1);
    }

    private void UpdateOptionIndex(int ammount) {
        print("navigate");
        int maxIndex = currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count - 1;
        optionsElement.text = "";
        if (dialogueOptionIndex + ammount > maxIndex)
        {
            dialogueOptionIndex = 0;
        }
        else if (dialogueOptionIndex + ammount < 0)
        {
            dialogueOptionIndex = maxIndex;
        }
        else
        {
            dialogueOptionIndex += ammount;
        }
        UpdateDialogueOptions();
    }

    private void DisplayDialogueOptions()
    {
        List<DialogueOption> dialogueOptions = currentDialogue.dialogueSentences[currentIndex].dialogueOptions;

        dialogueOptionIndex = 0;

        
        optionsElement.text = "";

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            optionsElement.text += i == dialogueOptionIndex ? $"[{dialogueOptions[i].option}]" : dialogueOptions[i].option;
            if (i != dialogueOptions.Count - 1) optionsElement.text += "     ";
        }
    }

    private void UpdateDialogueOptions()
    {
        List<DialogueOption> dialogueOptions = currentDialogue.dialogueSentences[currentIndex].dialogueOptions;
        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            optionsElement.text += i == dialogueOptionIndex ? $"[{dialogueOptions[i].option}]" : dialogueOptions[i].option;
            if (i != dialogueOptions.Count - 1) optionsElement.text += "     ";
        }
    }



    private void DisplayNextSentence()
    {
        int lastIndex = currentDialogue.dialogueSentences.Count - 1;
        if (currentIndex == lastIndex)
        {
            Destroy(gameObject);
            return;
        }
        currentIndex++;
        StartCoroutine(TypeSentence(currentDialogue.dialogueSentences[currentIndex].text));
    }

    IEnumerator TypeSentence(string sentence)
    {
        textElement.text = "";
        optionsElement.text = "";
        foreach (char character in sentence.ToCharArray())
        {
            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        if (currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) DisplayDialogueOptions();
    }
    private void CompleteSentence()
    {
        StopAllCoroutines();
        textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        if (currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) DisplayDialogueOptions();
    }

    private bool IsTyping()
    {
        return textElement.text != currentDialogue.dialogueSentences[currentIndex].text;
    }

    private bool AreDialogueOptionsVisable()
    {
        if (!IsTyping() && currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) return true;
        return false;
    }
}
