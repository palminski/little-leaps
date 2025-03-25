using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreLevelText : MonoBehaviour
{
    DialogueManager dialogueManager;
    [SerializeField] private string previousLevel = "";
    [SerializeField] private string targetLevel = "";

    public TriggerEvent prestigeEvent;
    public TriggerEvent healingEvent;
    public TriggerEvent maxHealthEvent;
    // Start is called before the first frame update
    void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
        Dialogue dialogueCopy = dialogueManager.currentDialogue.Clone();
        string startingText = dialogueManager.textElement.text;


        //         > [PREVIOUSLEVEL] Complete
        // > Current Score: [SCORE]


        // Score
        if (previousLevel.Length > 0 && GameController.Instance.Score > 0)
        {
            string previousLevelText = $@"
> {previousLevel} Complete
> Current Cognitive Data: {GameController.Instance.Score}
";
            startingText = startingText.Replace("[PREVIOUSLEVELTEXT]", previousLevelText);
        }
        else
        {
            startingText = startingText.Replace("[PREVIOUSLEVELTEXT]", "");

        }
        startingText = startingText.Replace("[NEXTLEVEL]", targetLevel);

        if (GlobalConstants.prestigeTime.ContainsKey(GameController.Instance.SessionPrestige))
        {
            float timeSet = GlobalConstants.prestigeTime[GameController.Instance.SessionPrestige];

                    startingText = startingText.Replace("[TIMER]", ConvertFloatToTime(timeSet));


        }


        startingText = startingText.Replace("[TIMERSTATUS]", GlobalConstants.prestigeText.ContainsKey(GameController.Instance.SessionPrestige) ? GlobalConstants.prestigeText[GameController.Instance.SessionPrestige] : "CRITICAL");
        

        DialogueSentence sentence = new DialogueSentence();
        sentence.text = startingText;
        sentence.dialogueOptions = new List<DialogueOption>();

        dialogueCopy.dialogueSentences.Clear();
        dialogueCopy.dialogueSentences.Add(sentence);
        // dialogueCopy.dialogueSentences.Insert(0, sentence);
        print(GameController.Instance.SessionInstability);
        if (
            GameController.Instance.SessionInstability >= 4 &&
            // GameController.Instance.SessionInstability >= 0 &&
            (GameController.Instance.SessionPrestige < GlobalConstants.highestAllowedPrestige || GameController.Instance.MaxHealth > GlobalConstants.lowestAllowedMaxHealth || GameController.Instance.SessionHealing > GlobalConstants.lowestAllowedHealingMultiplier)
        )
        {
            DialogueSentence instabilityOptionsSentence = new DialogueSentence();
            string rabitText = GetRabitText();
            instabilityOptionsSentence.text = $@"
Attunement Available - Message Start
----------------------------------------------------

RABIT able to Attune.
This will increase VNT collection rate, 
but takes system resources away from other systems.

{rabitText}


[OPTIONTEXT]





Please make selection:
----------------------------------------------------
";
            instabilityOptionsSentence.dialogueOptions = new List<DialogueOption>();

            DialogueOption noOption = new DialogueOption();
            noOption.option = "No Action";
            noOption.sideText = "Do not lower capabilities.\nVNT absorption rate remains unchanged";
            instabilityOptionsSentence.dialogueOptions.Add(noOption);

            if (GameController.Instance.SessionPrestige < GlobalConstants.highestAllowedPrestige)
            {
                DialogueOption prestigeOption = new DialogueOption();
                prestigeOption.option = "Decrease Timer";
                prestigeOption.sideText = "Decrease reset timer by 30 seconds.\nThis option will also increase VNT absorption by 50%.";
                if (prestigeEvent != null) prestigeOption.triggerEvent = prestigeEvent;
                instabilityOptionsSentence.dialogueOptions.Add(prestigeOption);
            }

            if (GameController.Instance.MaxHealth > GlobalConstants.lowestAllowedMaxHealth)
            {
                DialogueOption healthOption = new DialogueOption();
                healthOption.option = "Decrease Max EGO";
                healthOption.sideText = "Decrease RABIT max EGO.\nThis option will also increase VNT absorption by 50%.";

                if (maxHealthEvent != null) healthOption.triggerEvent = maxHealthEvent;

                instabilityOptionsSentence.dialogueOptions.Add(healthOption);
            }

            if (GameController.Instance.SessionHealing > GlobalConstants.lowestAllowedHealingMultiplier)
            {
                DialogueOption healingOption = new DialogueOption();
                healingOption.option = "Decrease Recovery";
                healingOption.sideText = "Collected VNTs provide less EGO recovery.\nThis option will also increase VNT absorption by 50%.";

                if (healingEvent != null) healingOption.triggerEvent = healingEvent;
                instabilityOptionsSentence.dialogueOptions.Add(healingOption);
            }

            



            dialogueCopy.dialogueSentences.Add(instabilityOptionsSentence);
            GameController.Instance.UpdateInstability(-10);

        }

        dialogueManager.currentDialogue = dialogueCopy;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (healingEvent)
        {
            healingEvent.OnEventRaised.AddListener(OnHealingEventRaised);
        }
        if (maxHealthEvent)
        {
            maxHealthEvent.OnEventRaised.AddListener(OnMaxHealthEventRaised);
        }
        if (prestigeEvent)
        {
            prestigeEvent.OnEventRaised.AddListener(OnPrestigeEventRaised);
        }
    }

    private void OnDisable()
    {
        if (healingEvent)
        {
            healingEvent.OnEventRaised.RemoveListener(OnHealingEventRaised);
        }
        if (maxHealthEvent)
        {
            maxHealthEvent.OnEventRaised.RemoveListener(OnMaxHealthEventRaised);
        }
        if (prestigeEvent)
        {
            prestigeEvent.OnEventRaised.RemoveListener(OnPrestigeEventRaised);
        }
    }

    private void OnHealingEventRaised()
    {
        float newHealing;
        switch (GameController.Instance.SessionHealing)
        {
            case 50f:
                newHealing = 30f;
                break;

            case 30f:
                newHealing = 20f;
                break;

            case 20f:
                newHealing = 10f;
                break;

            default:
                newHealing = 10f;
                break;
        }

        GameController.Instance.UpdateMultiplier(GameController.Instance.SessionPrestige, GameController.Instance.MaxHealth, newHealing);
    }

    private void OnMaxHealthEventRaised()
    {
        int newMaxHealth = Mathf.Clamp(GameController.Instance.MaxHealth - 1, GlobalConstants.lowestAllowedMaxHealth, 8);
        GameController.Instance.UpdateMultiplier(GameController.Instance.SessionPrestige, newMaxHealth, GameController.Instance.SessionHealing);
    }

    private void OnPrestigeEventRaised()
    {
        int newPrestige = Mathf.Clamp(GameController.Instance.SessionPrestige + 1, 0, GlobalConstants.highestAllowedPrestige);
        GameController.Instance.UpdateMultiplier(newPrestige, GameController.Instance.MaxHealth, GameController.Instance.SessionHealing);
    }

    private string ConvertFloatToTime(float currentTimer)
    {
        if (currentTimer <= 0) return "00:00:00";
        int totalSeconds = (int)currentTimer;
        float minutes = Mathf.Floor(totalSeconds / 60);
        int seconds = totalSeconds % 60;
        int hundredths = (int)(Mathf.Abs(totalSeconds - currentTimer) * 100);
        return minutes + ":" + seconds.ToString("D2") + ":" + hundredths.ToString("D2");
    }

    private string GetRabitText()
    {
        string[] options = {
            "> Hey Boss! Looks like we are doing pretty good!\n> What do you think? Should we go for it?",
            "> Hmmm! An interesting opportunity!\n> I bet we can push ourselved a bit harder",
            "> Dang, we're on fire!\n> Let's Knock it up a notch! BAM!!!",
            "> Hmmm! An interesting opportunity!\n> I'm down to make things a bit harder if you are!",
            "> This will raise the steaks a bit boss!\n> But I'm sure its nothing we can't handle.",
            "> I wouldn't let this go to waste.\n> No preassure though.",
        };


        return options[Random.Range(0,options.Length)];
    }

    
}
