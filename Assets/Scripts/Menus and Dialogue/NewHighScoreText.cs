using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NewHighScoreText : MonoBehaviour
{
    [SerializeField] public float timeBetweenCharacters = 0.05f;

    [SerializeField] private TMP_Text textElement;

    [SerializeField] private GameObject objectToActivateUponCompletion;

    [SerializeField] private NewHighScoreController highScoreConttroller;

    private string startingText;
    public bool isFinishedTyping = false;
    // Start is called before the first frame update
    void Awake()
    {

        textElement = GetComponent<TMP_Text>();
        // startingText = textElement.text;
        startingText = textElement.text;
        



        StartCoroutine(TypeSentence(startingText));

    }

    IEnumerator WaitAndCompleteText()
    {
        yield return new WaitForSeconds(0.05f);
        CompleteText();
    }

    

    IEnumerator TypeSentence(string textToType)
    {
        textElement.text = "";
        foreach (char character in textToType.ToCharArray())
        {
            textElement.text += character;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
        CompleteText();
    }

    void CompleteText()
    {
        objectToActivateUponCompletion.SetActive(true);
        if (highScoreConttroller) highScoreConttroller.canEnter = true;
        isFinishedTyping = true;
        Destroy(GetComponent<PlayerInput>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFinishedTyping && Input.anyKeyDown)
        {
            StopAllCoroutines();
            textElement.text = startingText;
            if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
            StartCoroutine(WaitAndCompleteText());
        }
    }
}
