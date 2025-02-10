using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.VisualScripting.Dependencies.NCalc;
using System.Linq;

[Serializable]
public class MenuOption
{
    public UnityEvent action;
    public string levelKey;
    public string requiredKey;
    public string text;

    [TextArea(15, 15)] public string sideText;

}

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TriggerEvent eventToTrigger;
    [SerializeField] private TriggerEvent secondEventToTrigger;
    public TMP_Text currentText;
    public TMP_Text mainMenu;
    public TMP_Text levelSelectMenu;
    public TMP_Text timeSelectMenu;
    public TMP_Text maxLivesMenu;
    public TMP_Text healingMenu;
    public TMP_Text controlMenu;
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] currentOptions;
    public MenuOption[] menuOptions;
    public MenuOption[] levelSelectOptions;
    public MenuOption[] timeSelectOptions;
    public MenuOption[] maxLivesOptions;
    public MenuOption[] healingOptions;
    public MenuOption[] controlOptions;
    public StartingText startingText;
    private List<string> permCollected;
    public Dictionary<string, MenuOption> levelDictionary;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        gameObject.SetActive(false);

        permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;
        menuOptions = menuOptions.Where(option => option.requiredKey.Length == 0 || permCollected.Contains(option.requiredKey)).ToArray();
        levelSelectOptions = levelSelectOptions.Where(option => option.requiredKey.Length == 0 || permCollected.Contains(option.requiredKey)).ToArray();
    }

    void OnEnable()
    {
        SetCurrentMenu(menuOptions, mainMenu);
        UpdateMenuText();

    }

    public void StartGame()
    {
        SetCurrentMenu(levelSelectOptions, levelSelectMenu);
    }

    public void AdjustTime()
    {
        SetCurrentMenu(timeSelectOptions, timeSelectMenu);
    }
    public void AdjustLives()
    {
        SetCurrentMenu(maxLivesOptions, maxLivesMenu);
    }

    public void AdjustHealing()
    {
        SetCurrentMenu(healingOptions, healingMenu);
    }

    public void AdjustControls()
    {
        SetCurrentMenu(controlOptions, controlMenu);
    }

    public void StartLevel(string levelToStart)
    {
        if (eventToTrigger != null)
        {
            eventToTrigger.Raise();
        }
        LevelConnection.ActiveConnection = null;
        GameController.Instance.ResetGameState();
        GameController.Instance.SetCheckPoint("Main Menu");
        StartCoroutine(WaitAndChangeScene(levelToStart));

    }

    IEnumerator WaitAndChangeScene(string levelToStart)
    {

        yield return new WaitForSeconds(0.5f);
        if (secondEventToTrigger != null)
        {
            secondEventToTrigger.Raise();
        }
        yield return new WaitForSeconds(0.75f);

        GameController.Instance.ChangeScene(levelToStart);

    }
    public void Debug()
    {
        print("Debug");
        SaveDataManager.DeleteGameData();
        GameController.Instance.ChangeScene("Main Menu");

    }

    public void Shift()
    {
        GameController.Instance.ToggleRoomState();
    }

    public void ExitGame()
    {
        print("Exit Game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // ==============================================================================
    // Menu Navigation
    // ==============================================================================
    void OnNavigateRight()
    {
        UpdateIndex(1);
    }

    void OnNavigateLeft()
    {
        UpdateIndex(-1);
    }

    void OnNavigateDown()
    {
        UpdateIndex(1);
    }

    void OnNavigateUp()
    {
        UpdateIndex(-1);
    }

    void OnSelect()
    {
        currentOptions[selectedIndex].action.Invoke();
    }
    void OnBack()
    {
        SetCurrentMenu(menuOptions, mainMenu);
    }

    void OnToggleRoom()
    {
        Shift();
    }

    public void SelectPrestige(int prestigeToSet)
    {
        SaveData saveData = SaveDataManager.LoadGameData();
        saveData.prestige = prestigeToSet;
        SaveDataManager.SaveGameData(saveData);
        SetCurrentMenu(menuOptions, mainMenu);
        GameController.Instance.ResetGameState();
        startingText.UpdateText(true);
    }

    public void SelectMaxLives(int maxLivesToSet)
    {
        SaveData saveData = SaveDataManager.LoadGameData();
        saveData.maxLives = maxLivesToSet;
        SaveDataManager.SaveGameData(saveData);
        SetCurrentMenu(menuOptions, mainMenu);
        GameController.Instance.ResetGameState();
        startingText.UpdateText(true);
    }

    public void SelectHealing(float healingToSet)
    {
        SaveData saveData = SaveDataManager.LoadGameData();
        saveData.healing = healingToSet;
        SaveDataManager.SaveGameData(saveData);
        SetCurrentMenu(menuOptions, mainMenu);
        GameController.Instance.ResetGameState();
        startingText.UpdateText(true);
    }

    private void UpdateIndex(int ammount)
    {
        int maxIndex = currentOptions.Length - 1;
        if (selectedIndex + ammount > maxIndex)
        {
            selectedIndex = 0;
        }
        else if (selectedIndex + ammount < 0)
        {
            selectedIndex = maxIndex;
        }
        else
        {
            selectedIndex += ammount;
        }
        UpdateMenuText();
    }

    private void UpdateMenuText()
    {
        currentText.text = "";
        foreach (MenuOption menuOption in currentOptions)
        {
            string textLine = "";
            if (currentOptions[selectedIndex] == menuOption)
            {
                textLine += "[";
            }
            else
            {
                textLine += " ";
            }
            textLine += menuOption.text;
            if (currentOptions[selectedIndex] == menuOption)
            {
                textLine += "]";
            }
            textLine += "\n";

            currentText.text += textLine;
        }
        if (startingText.isFinishedTyping) startingText.SetSideText(ParseSideText(currentOptions[selectedIndex]));
    }

    private void SetCurrentMenu(MenuOption[] newMenuOptions, TMP_Text newMenuText)
    {
        DisableMenus();
        // if (newMenuOptions == menuOptions)
        // {
        //     startingText.gameObject.SetActive(true);
        // }
        newMenuText.gameObject.SetActive(true);
        currentText = newMenuText;
        currentOptions = newMenuOptions;
        selectedIndex = 0;
        UpdateMenuText();
    }

    private void DisableMenus()
    {
        // startingText.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        if (levelSelectMenu) levelSelectMenu.gameObject.SetActive(false);
        if (timeSelectMenu) timeSelectMenu.gameObject.SetActive(false);
        if (maxLivesMenu) maxLivesMenu.gameObject.SetActive(false);
        if (healingMenu) healingMenu.gameObject.SetActive(false);
        if (controlMenu) controlMenu.gameObject.SetActive(false);
    }

    private string ParseSideText(MenuOption option)
    {
        string textToParse = option.sideText;
        if (option.levelKey.Length > 0)
        {
            SaveData saveData = SaveDataManager.LoadGameData();



            //Write out best time for sector
            BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == option.levelKey);
            if (existingTime != null)
            {
                textToParse = option.sideText.Replace("[BEST TIME]", ConvertFloatToTime(existingTime.time));
            }
            textToParse = textToParse.Replace("[BEST TIME]", "INCOMPLETE");

            //Write out best score for sector
            BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == option.levelKey);
            if (existingScore != null)
            {
                int bestScore = (int)existingScore.score;
                textToParse = textToParse.Replace("[BEST SCORE]", bestScore.ToString());

                //deal with score thresholds
                if (GlobalConstants.levelScoreThresholds.ContainsKey(option.levelKey))
                {
                    SortedDictionary<int, string> thresholds = GlobalConstants.levelScoreThresholds[option.levelKey];
                    string textToDisplay = "";
                    bool exitedEarly = false;

                    foreach (var kvp in thresholds)
                    {
                        if (kvp.Key <= bestScore) {
                            textToDisplay += $"{kvp.Value}\n";
                        } else {
                            textToDisplay += $"[{kvp.Key} Required To Decrypt]";
                            exitedEarly = true;
                            break;
                        }
                    }
                    if (!exitedEarly) {
                        textToDisplay += $"[SECTOR FULLY DECRYPTED!]";
                    }
                    textToParse = textToParse.Replace("[SCORE THRESHOLDS]", textToDisplay);
                }
            }
            textToParse = textToParse.Replace("[BEST SCORE]", "INCOMPLETE");








        }
        return textToParse;
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
}
