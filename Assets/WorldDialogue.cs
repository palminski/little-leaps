using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WorldDialogue : MonoBehaviour
{
    private TMP_Text textElement;
    private Collider2D thisCollider;
    private Coroutine typingCoroutine;
    private Coroutine unTypingCoroutine;
    [SerializeField] private float timeBetweenCharacters = 0.05f;
    private string textToType;
    private GameObject player;
    [SerializeField] private Interactable interactable;

    private int currentCharacterIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponent<TMP_Text>();
        thisCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        textToType = textElement.text;
        textElement.text = "";
        if (thisCollider == null)
        {
            StartCoroutine(TypeSentence());
        }
    }

    void Update()
    {
        if (!interactable) return;
        if (interactable.CanInteractWith())
        {
            
            if (unTypingCoroutine != null)
            {
                StopCoroutine(unTypingCoroutine);
                unTypingCoroutine = null;
            }
            if(typingCoroutine != null) return;
            typingCoroutine = StartCoroutine(TypeSentence());
        }
        else
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            if(unTypingCoroutine != null) return;
            unTypingCoroutine = StartCoroutine(UnTypeSentence());
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (interactable == null && collider.gameObject == player)
        {
            if (unTypingCoroutine != null)
            {
                StopCoroutine(unTypingCoroutine);
                unTypingCoroutine = null;
            }
            typingCoroutine = StartCoroutine(TypeSentence());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (interactable == null && collider.gameObject == player)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            unTypingCoroutine = StartCoroutine(UnTypeSentence());
        }
    }

    IEnumerator TypeSentence()
    {
        for (int i = currentCharacterIndex; i < textToType.Length; i++)
        {
            textElement.text += textToType[i];
            currentCharacterIndex ++;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }

    IEnumerator UnTypeSentence()
    {
        while (currentCharacterIndex > 0)
        {
            currentCharacterIndex --;
            textElement.text = textToType.Substring(0,currentCharacterIndex);
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }
    
}
