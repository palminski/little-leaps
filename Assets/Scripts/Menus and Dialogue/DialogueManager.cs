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

    public bool ahouldDispayHorizontal = false;

    private Coroutine typingCoroutine;

    // Start is called before the first frame update
    void Start()
    {

        currentIndex = 0;
        typingCoroutine = StartCoroutine(TypeSentence(SwapInSideText(currentDialogue.dialogueSentences[currentIndex].text)));
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            playerInput.enabled = false;
        }


    }

    void OnDestroy()
    {
        if (playerInput) playerInput.enabled = true;
        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
        }
    }

    void OnSelect()
    {
        // textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        if (IsTyping())
        {
            CompleteSentence();
            return;
        }

        if (AreDialogueOptionsVisable())
        {
            DialogueOption dialogueOption = currentDialogue.dialogueSentences[currentIndex].dialogueOptions[dialogueOptionIndex];

            if (dialogueOption.nextDialogue)
            {
                currentDialogue = dialogueOption.nextDialogue;
                currentIndex = 0;
                typingCoroutine = StartCoroutine(TypeSentence(SwapInSideText(currentDialogue.dialogueSentences[currentIndex].text)));
                return;
            }

            if (dialogueOption.optionEvent != null)
            {
                dialogueOption.optionEvent.Raise();
            }

            if (dialogueOption.triggerEvent != null)
            {
                dialogueOption.triggerEvent.Raise();
            }
        }
        DisplayNextSentence();
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
    }

    void OnNavigateLeft()
    {

        // textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateUp()
    {
        // textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateRight()
    {
        // textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateDown()
    {
        // textElement.text = currentDialogue.dialogueSentences[currentIndex].text;
        if (AreDialogueOptionsVisable()) UpdateOptionIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    private void UpdateOptionIndex(int ammount)
    {
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
        UpdateWithSideText();

        UpdateDialogueOptions();
    }

    private void UpdateWithSideText()
    {
        if (AreDialogueOptionsVisable())
        {
            DialogueOption dialogueOption = currentDialogue.dialogueSentences[currentIndex].dialogueOptions[dialogueOptionIndex];

            if (dialogueOption.sideText != "")
            {
                string newText = currentDialogue.dialogueSentences[currentIndex].text;
                newText = newText.Replace("[OPTIONTEXT]", dialogueOption.sideText);
                textElement.text = newText;
            }
            else
            {
                string newText = currentDialogue.dialogueSentences[currentIndex].text;
                newText = newText.Replace("[OPTIONTEXT]", "");
                textElement.text = newText;
            }
        }
    }

    private string SwapInSideText(string text)
    {
        // PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        if (InputController.Instance != null && text.Contains("[BINDING]"))
        {
            
                string currentControlScheme = (InputController.Instance != null) ? InputController.Instance.GetLastUsedDevice() : "";
                text = text.Replace("[BINDING]", currentControlScheme == "Gamepad" ? "[ Button South ]" : "[ Enter ]") ;
            
        }
        if (currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0)
        {
            DialogueOption dialogueOption = currentDialogue.dialogueSentences[currentIndex].dialogueOptions[dialogueOptionIndex];

            if (dialogueOption.sideText != "")
            {
                return text.Replace("[OPTIONTEXT]", dialogueOption.sideText);
            }
            else
            {
                return text.Replace("[OPTIONTEXT]", "");
            }
        }

        
        


        return text;
    }


    private void DisplayDialogueOptions()
    {
        List<DialogueOption> dialogueOptions = currentDialogue.dialogueSentences[currentIndex].dialogueOptions;

        dialogueOptionIndex = 0;


        optionsElement.text = "";

        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            optionsElement.text += i == dialogueOptionIndex ? $"[{dialogueOptions[i].option}]" : dialogueOptions[i].option;
            if (i != dialogueOptions.Count - 1) optionsElement.text += ahouldDispayHorizontal ? "     " : "\n";
        }
    }

    private void UpdateDialogueOptions()
    {
        List<DialogueOption> dialogueOptions = currentDialogue.dialogueSentences[currentIndex].dialogueOptions;
        for (int i = 0; i < dialogueOptions.Count; i++)
        {
            optionsElement.text += i == dialogueOptionIndex ? $"[{dialogueOptions[i].option}]" : dialogueOptions[i].option;
            if (i != dialogueOptions.Count - 1) optionsElement.text += ahouldDispayHorizontal ? "     " : "\n";
        }
    }



    private void DisplayNextSentence()
    {
        int lastIndex = currentDialogue.dialogueSentences.Count - 1;
        if (currentIndex == lastIndex)
        {
            Destroy(gameObject);
            ChangeRoomAfterDialogue changeRoomAfterDialogue = GetComponent<ChangeRoomAfterDialogue>();
            if (changeRoomAfterDialogue != null)
            {
                changeRoomAfterDialogue.ChangeRoom();
            }
            return;
        }
        currentIndex++;
        typingCoroutine = StartCoroutine(TypeSentence(SwapInSideText(currentDialogue.dialogueSentences[currentIndex].text)));
    }

    IEnumerator TypeSentence(string sentence)
    {
        textElement.text = "";
        optionsElement.text = "";
        foreach (char character in sentence.ToCharArray())
        {
            if (character != ' ') if (AudioController.Instance != null) AudioController.Instance.PlayTypingBeep();

            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        if (currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) DisplayDialogueOptions();
        typingCoroutine = null;
    }
    private void CompleteSentence()
    {
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
        StopAllCoroutines();
        typingCoroutine = null;
        textElement.text = SwapInSideText(currentDialogue.dialogueSentences[currentIndex].text);
        if (currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) DisplayDialogueOptions();
    }

    private bool IsTyping()
    {
        // string textToCompare = currentDialogue.dialogueSentences[currentIndex].text;
        // return textElement.text != textToCompare;
        return typingCoroutine != null;
    }

    private bool AreDialogueOptionsVisable()
    {
        if (!IsTyping() && currentDialogue.dialogueSentences[currentIndex].dialogueOptions.Count > 0) return true;
        return false;
    }
}
