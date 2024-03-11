using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text textElement;

    [SerializeField]
    public float timeBetweenCharacters = 0.05f;

    public Dialogue currentDialogue;

    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponentInChildren<TMP_Text>();
        currentIndex = 0;
        StartCoroutine(TypeSentence(currentDialogue.dialogueSentences[currentIndex].text));
    }

    void OnSelect()
    {
        if (IsTyping()) {
            CompleteSentence();
            return;
        }
        DisplayNextSentence();
    }


    private void DisplayNextSentence() {
        int lastIndex = currentDialogue.dialogueSentences.Count - 1;
        if (currentIndex == lastIndex) {
            Destroy(gameObject);
            return;
        }
        currentIndex++;
        StartCoroutine(TypeSentence(currentDialogue.dialogueSentences[currentIndex].text));
    }

    IEnumerator TypeSentence(string sentence) {
        textElement.text = "";
        foreach(char character in sentence.ToCharArray())
        {
            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }
    private void CompleteSentence() {
        StopAllCoroutines();
        textElement.text = currentDialogue.dialogueSentences[currentIndex].text; 
    }

    private bool IsTyping() {
        return textElement.text != currentDialogue.dialogueSentences[currentIndex].text;
    }
}
