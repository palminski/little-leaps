using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class WorldDialogue : MonoBehaviour
{
    public TMP_Text textElement;
    private Collider2D thisCollider;
    private Coroutine typingCoroutine;
    private Coroutine unTypingCoroutine;
    [SerializeField] private float timeBetweenCharacters = 0.05f;
    public string startingText;
    public string textToType;
    private GameObject player;
    [SerializeField] private Interactable interactable;

    [SerializeField] private TriggerEvent activateOnEvent;
    [SerializeField] private TriggerEvent disableOnEvent;
    private string id;

    private int currentCharacterIndex = 0;
    [SerializeField] public bool shouldShowOnce;

    private void OnEnable()
    {
        if (activateOnEvent)
        {
            activateOnEvent.OnEventRaised.AddListener(Activate);
        }
        if (disableOnEvent)
        {
            disableOnEvent.OnEventRaised.AddListener(Disable);
        }
    }
    private void OnDisable()
    {
        if (activateOnEvent)
        {
            activateOnEvent.OnEventRaised.RemoveListener(Activate);
        }
        if (disableOnEvent)
        {
            disableOnEvent.OnEventRaised.RemoveListener(Disable);
        }
    }

    void Start()
    {
        id = $"dialogue-{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (disableOnEvent != null && GameController.Instance.CollectedObjects.Contains(id)) Destroy(gameObject);
        textElement = GetComponent<TMP_Text>();
        thisCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        startingText = textElement.text;

        textToType = shouldShowOnce || (activateOnEvent && !GameController.Instance.CollectedObjects.Contains(id)) ? "" : startingText;
        
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
            if(player && player.activeSelf || currentCharacterIndex < textToType.Length-1) {
                currentCharacterIndex --;
                textElement.text = textToType.Substring(0,currentCharacterIndex);
            }
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }

    void Activate()
    {
        GameController.Instance.CollectedObjects.Add(id);

        textElement.text = "";
        textToType = startingText;
        if (unTypingCoroutine != null)
        {
            StopCoroutine(unTypingCoroutine);
            unTypingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(TypeSentence());
    }

    void Disable()
    {
        GameController.Instance.CollectedObjects.Add(id);
        Destroy(gameObject);
    }
    
}
