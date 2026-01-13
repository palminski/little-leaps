using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ParagraphInEditor
{
    [TextArea(3,10)]
    public string text;
}
public class TalkingNPCTextSwapper : MonoBehaviour
{
    public List<ParagraphInEditor> textOptions = new List<ParagraphInEditor>();
    private WorldDialogue worldDialogue;

    void Awake()
    {
        worldDialogue = GetComponent<WorldDialogue>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textOptions.Count <= 0) return;
        int index = Random.Range(0, textOptions.Count);
        worldDialogue.textToType = textOptions[index].text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
