using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchUntilPointThreshold : MonoBehaviour
{
    [SerializeField] private int requiredPoints = 0;
    [SerializeField] private float minimumPercent = 0.8f;
    WorldDialogue dialogue;


    // Start is called before the first frame update
    void Start()
    {
        if (requiredPoints == 0) return;

        dialogue = GetComponent<WorldDialogue>();
        string message = dialogue.textToType;
        int points = GameController.Instance.Score;

        float glitchChance = Mathf.Min(minimumPercent, 1f - Mathf.Clamp01(points / requiredPoints));

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
        dialogue.textToType = finalMessage;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
