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
using System.Text.RegularExpressions;
using UnityEngine.TextCore.Text;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;

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
    // public TMP_Text timeSelectMenu;
    // public TMP_Text maxLivesMenu;
    // public TMP_Text healingMenu;
    public TMP_Text controlMenu;
    public TMP_Text graphicMenu;
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] currentOptions;
    public MenuOption[] menuOptions;
    public MenuOption[] levelSelectOptions;
    // public MenuOption[] timeSelectOptions;
    // public MenuOption[] maxLivesOptions;
    // public MenuOption[] healingOptions;
    public MenuOption[] controlOptions;
    public MenuOption[] graphicOptions;
    public StartingText startingText;
    private List<string> permCollected;
    public Dictionary<string, MenuOption> levelDictionary;

    //=======================================
    private SteamLeaderboard_t leaderboard;
    private bool leaderboardReady = false;
    private int localScore;
    private CallResult<LeaderboardFindResult_t> findResult;
    private CallResult<LeaderboardScoreUploaded_t> uploadResult;
    private CallResult<LeaderboardScoresDownloaded_t> downloadResult;
    private string leaderboardText = "loading Leaderboard...";

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.LoadBinding(playerInput);
        }

        selectedIndex = 0;
        // gameObject.SetActive(false);

        permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;
        menuOptions = menuOptions.Where(option => option.requiredKey.Length == 0 || permCollected.Contains(option.requiredKey)).ToArray();
        levelSelectOptions = levelSelectOptions.Where(option => option.requiredKey.Length == 0 || permCollected.Contains(option.requiredKey)).ToArray();

        GameController.Instance.SetShouldSkipGameover(false);

        ManageHighScores();

    }

    void OnEnable()
    {
        SetCurrentMenu(menuOptions, mainMenu);
        UpdateMenuText();
        InputController.Instance.OnFinishedRebind += HandleFinishedRebind;
        InputController.Instance.OnRestoredToDefault += HandleFinishedBindingReset;

    }

    private void OnDisable()
    {
        InputController.Instance.OnFinishedRebind -= HandleFinishedRebind;
        InputController.Instance.OnRestoredToDefault -= HandleFinishedBindingReset;

        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
        }
    }

    public void StartGame()
    {
        // SetCurrentMenu(levelSelectOptions, levelSelectMenu);
        StartLevel("lv_0_start");
    }

    public void SelectLevel()
    {
        SetCurrentMenu(levelSelectOptions, levelSelectMenu);
    }

    // public void AdjustTime()
    // {
    //     SetCurrentMenu(timeSelectOptions, timeSelectMenu);
    // }
    // public void AdjustLives()
    // {
    //     SetCurrentMenu(maxLivesOptions, maxLivesMenu);
    // }

    // public void AdjustHealing()
    // {
    //     SetCurrentMenu(healingOptions, healingMenu);
    // }

    public void AdjustControls()
    {
        SetCurrentMenu(controlOptions, controlMenu);
    }

    public void AdjustGraphics()
    {
        SetCurrentMenu(graphicOptions, graphicMenu);
    }

    public void StartLevel(string levelToStart)
    {
        if (playerInput != null && InputController.Instance != null)
        {
            InputController.Instance.SetLastUsedDevice(playerInput.currentControlScheme);
            playerInput.enabled = false;
        }
        LevelConnection.ActiveConnection = null;
        GameController.Instance.ResetGameState();

        var permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;

        if (permCollected.Contains("RABIT escaped"))
        {
            GameController.Instance.SetCheckPoint(levelToStart);
            GameController.Instance.ChangeScene("RABIT Return");
            return;
        }
        if (eventToTrigger != null)
        {
            eventToTrigger.Raise();
        }
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

    public void PrestigeArea()
    {
        GameController.Instance.SetCheckPointBackend("Main Menu");
        GameController.Instance.ChangeScene("maze_end");
    }

    // public void Debug()
    // {
    //     SaveDataManager.DeleteGameData();
    //     PlayerPrefs.DeleteAll();
    //     GameController.Instance.ChangeScene("Main Menu");

    // }

    public void Shift()
    {
        GameController.Instance.ToggleRoomState();
    }

    public void ExitGame()
    {
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
        if (!startingText.isFinishedTyping) return;
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateLeft()
    {
        if (!startingText.isFinishedTyping) return;
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateDown()
    {
        if (!startingText.isFinishedTyping) return;
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateUp()
    {
        if (!startingText.isFinishedTyping) return;
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnSelect()
    {
        if (!startingText.isFinishedTyping)
        {
            startingText.ActivateAndCompleteText();
            return;
        }
        
        currentOptions[selectedIndex].action.Invoke();
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
    }
    void OnBack()
    {
        if (!startingText.isFinishedTyping) return;
        SetCurrentMenu(menuOptions, mainMenu);
        if (AudioController.Instance != null) AudioController.Instance.PlayBack();
    }

    void OnToggleRoom()
    {
        Shift();
        if (AudioController.Instance != null) AudioController.Instance.PlayShift();

    }

    // public void SelectPrestige(int prestigeToSet)
    // {
    //     SaveData saveData = SaveDataManager.LoadGameData();
    //     saveData.prestige = prestigeToSet;
    //     SaveDataManager.SaveGameData(saveData);
    //     SetCurrentMenu(menuOptions, mainMenu);
    //     GameController.Instance.ResetGameState();
    //     startingText.UpdateText(true);
    // }

    // public void SelectMaxLives(int maxLivesToSet)
    // {
    //     SaveData saveData = SaveDataManager.LoadGameData();
    //     saveData.maxLives = maxLivesToSet;
    //     SaveDataManager.SaveGameData(saveData);
    //     SetCurrentMenu(menuOptions, mainMenu);
    //     GameController.Instance.ResetGameState();
    //     startingText.UpdateText(true);
    // }

    // public void SelectHealing(float healingToSet)
    // {
    //     SaveData saveData = SaveDataManager.LoadGameData();
    //     saveData.healing = healingToSet;
    //     SaveDataManager.SaveGameData(saveData);
    //     SetCurrentMenu(menuOptions, mainMenu);
    //     GameController.Instance.ResetGameState();
    //     startingText.UpdateText(true);
    // }

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
        // if (timeSelectMenu) timeSelectMenu.gameObject.SetActive(false);
        // if (maxLivesMenu) maxLivesMenu.gameObject.SetActive(false);
        // if (healingMenu) healingMenu.gameObject.SetActive(false);
        if (controlMenu) controlMenu.gameObject.SetActive(false);
        if (graphicMenu) graphicMenu.gameObject.SetActive(false);
    }

    private string ParseSideText(MenuOption option)
    {
        string textToParse = option.sideText;
        if (option.levelKey.Length > 0)
        {
            SaveData saveData = SaveDataManager.LoadGameData();



            //Write out best time for sector =======================================================================
            BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == option.levelKey);
            if (existingTime != null)
            {
                textToParse = option.sideText.Replace("[BEST TIME]", ConvertFloatToTime(existingTime.time));
            }
            textToParse = textToParse.Replace("[BEST TIME]", "INCOMPLETE");

            //Write out best score for sector ========================================================================
            BestScore existingScore = saveData.bestScores.FirstOrDefault(score => score.level == option.levelKey);
            if (existingScore != null)
            {
                int bestScore = (int)existingScore.score;
                textToParse = textToParse.Replace("[BEST SCORE]", bestScore.ToString());

            }
            textToParse = textToParse.Replace("[BEST SCORE]", "INCOMPLETE");


            BestChips existingChips = saveData.bestChips.FirstOrDefault(existingChip => existingChip.level == option.levelKey);
            string textToDisplay = "";
            //deal with score thresholds
            if (GlobalConstants.levelScoreThresholds.ContainsKey(option.levelKey))
            {
                SortedDictionary<int, string> thresholds = GlobalConstants.levelScoreThresholds[option.levelKey];

                // bool exitedEarly = false;

                foreach (var kvp in thresholds)
                {
                    if (existingChips != null && kvp.Key <= existingChips.chips)
                    {
                        textToDisplay += $"{kvp.Value}\n";
                    }
                    else
                    {
                        textToDisplay += $"-- {kvp.Key} Node{(kvp.Key != 1 ? 's' : ' ')} Required --\n\n";
                        // exitedEarly = true;
                        // break;
                    }
                }
                // if (!exitedEarly)
                // {
                //     textToDisplay += $"[SECTOR FULLY DECRYPTED!]";
                // }
                int lineCount = Regex.Split(textToDisplay, @"\r\n|\r|\n").Length;
                for (int i = lineCount; i < 15; i++)
                {
                    textToDisplay += "\n";
                }
                textToParse = textToParse.Replace("[SCORE THRESHOLDS]", textToDisplay);
            }

            if (option.sideText.Contains("[FPS]"))
            {
                textToParse = textToParse.Replace("[FPS]", "[ " + (PlayerPrefs.HasKey("TargetFPS") ? PlayerPrefs.GetInt("TargetFPS") : "Uncapped") + " ]");

            }

            if (option.sideText.Contains("[LEADERBOARD]"))
            {
                textToParse = textToParse.Replace("[LEADERBOARD]", leaderboardText);

            }

            //Write Out Text For Controls
            // BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == option.levelKey);
            if (option.sideText.Contains("[BINDING]"))
            {
                var action = playerInput.actions.FindAction(option.levelKey);
                if (action != null)
                {
                    string currentControlScheme = playerInput.currentControlScheme;
                    if (!string.IsNullOrEmpty(currentControlScheme))
                    {
                        foreach (var binding in action.bindings)
                        {
                            if (binding.path.Contains(currentControlScheme == "Gamepad" ? "Gamepad" : "Keyboard"))
                            {
                                textToParse = textToParse.Replace("[BINDING]", "[ " + AdjustAndGetInputString(currentControlScheme, binding.ToDisplayString()) + " ]");
                                break;
                            }
                        }
                    }
                }
            }



        }
        return textToParse;
    }

    string AdjustAndGetInputString(string currentControlScheme, string bindingString)
    {
        if (currentControlScheme != "Gamepad")
        {
            return bindingString;
        }
        if (bindingString == "A")
        {
            return "Button South";
        }
        if (bindingString == "B")
        {
            return "Button East";
        }
        if (bindingString == "X")
        {
            return "Button West";
        }
        if (bindingString == "Y")
        {
            return "Button North";
        }
        return bindingString;
    }

    public void SetSideText(string newSideText)
    {
        startingText.SetSideText(newSideText);
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

    public void UnlockAllLevels()
    {

        SaveDataManager.AddPermanentCollectedString("lv_1_complete");
        SaveDataManager.AddPermanentCollectedString("lv_2_complete");
        SaveDataManager.AddPermanentCollectedString("lv_3_complete");
        SaveDataManager.AddPermanentCollectedString("lv_4_complete");
        SaveDataManager.AddPermanentCollectedString("lv_5_complete");
        SaveDataManager.AddPermanentCollectedString("lv_6_complete");
        SaveDataManager.AddPermanentCollectedString("lv_7_complete");
        SaveDataManager.AddPermanentCollectedString("lv_8_complete");
        SaveDataManager.AddPermanentCollectedString("lv_9_complete");
        SaveDataManager.AddPermanentCollectedString("lv_10_complete");
        SaveDataManager.AddPermanentCollectedString("lv_11_complete");
        SaveDataManager.AddPermanentCollectedString("lv_12_complete");
        SaveDataManager.AddPermanentCollectedString("lv_13_complete");
        SaveDataManager.AddPermanentCollectedString("lv_14_complete");
        SaveDataManager.AddPermanentCollectedString("lv_15_complete");
        SaveDataManager.AddPermanentCollectedString("lv_16_complete");
        SaveDataManager.AddPermanentCollectedString("lv_17_complete");
        SaveDataManager.AddPermanentCollectedString("lv_18_complete");
        SaveDataManager.AddPermanentCollectedString("lv_19_complete");
        SaveDataManager.AddPermanentCollectedString("lv_20_complete");
        GameController.Instance.ChangeScene("Main Menu");
    }

    public void ToggleMusic()
    {

        AudioController.Instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioController.Instance.ToggleSFX();
    }

    public void ToggleSpeedRunTimer()
    {
        if (PlayerPrefs.HasKey("ShowSpeedRunTimer"))
        {
            PlayerPrefs.DeleteKey("ShowSpeedRunTimer");
            PlayerPrefs.Save();
            return;
        }
        PlayerPrefs.SetInt("ShowSpeedRunTimer",1);
        PlayerPrefs.Save();
    }

    public void RebindControl(string actionName)
    {
        // print(playerInput.currentControlScheme);
        // return;
        InputController.Instance.StartRebinding(actionName, playerInput.currentControlScheme != null ? playerInput.currentControlScheme : "Keyboard", updatedBinding =>
        {

        });
        SetSideText("Awaiting Input...");
    }

    public void ResetRebinds()
    {
        foreach (var action in playerInput.actions)
        {
            action.RemoveAllBindingOverrides();
        }
        InputController.Instance.ResetRebindings(playerInput.currentControlScheme != null ? playerInput.currentControlScheme : "Keyboard");
        InputController.Instance.LoadBinding(playerInput);
    }

    private void HandleFinishedRebind()
    {
        startingText.SetSideText(ParseSideText(currentOptions[selectedIndex]));
    }

    private void HandleFinishedBindingReset()
    {
        SetSideText("Controls Reset!");
    }

    public void SetFPS(int targetFPS)
    {
        if (targetFPS == 0)
        {
            if (PlayerPrefs.HasKey("TargetFPS"))
            {
                PlayerPrefs.DeleteKey("TargetFPS");
                PlayerPrefs.Save();
                if (startingText.isFinishedTyping) startingText.SetSideText(ParseSideText(currentOptions[selectedIndex]));
            }
            return;
        }
        PlayerPrefs.SetInt("TargetFPS", targetFPS);
        PlayerPrefs.Save();
        GameController.Instance.ApplyGraphicsSettings();
        if (startingText.isFinishedTyping) startingText.SetSideText(ParseSideText(currentOptions[selectedIndex]));
    }

    // ========================================
    private void ManageHighScores()
    {
        if (SteamManager.Initialized)
        {
            // My Highest Score
            SaveData gameData = SaveDataManager.LoadGameData();
            gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
            HighScore highScore = gameData.highScores.First();
            int score = highScore.score;

            string name = SteamFriends.GetPersonaName();
            localScore = score;

            //Call leaderboard
            SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(
                "Top VNT Yields",
                ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
            );

            findResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFound);
            findResult.Set(handle);
        }
    }

    void OnLeaderboardFound(LeaderboardFindResult_t result, bool bIOFailure)
    {
        if (bIOFailure)
        {
            Debug.LogError("Steam I/O Failure while finding leaderboard");
            leaderboardText = "leaderboard could not be loaded.";
            return;
        }
        if (result.m_bLeaderboardFound == 0)
        {
            Debug.LogError("Leaderboard not found and could not be created");
            leaderboardText = "leaderboard could not be loaded.";
            return;
        }
        leaderboard = result.m_hSteamLeaderboard;
        leaderboardReady = true;
        UploadScore(localScore);

    }

    void UploadScore(int score)
    {
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(
            leaderboard,
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest,
            score,
            null,
            0
        );

        uploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnScoreUploaded);
        uploadResult.Set(handle);
    }

    void OnScoreUploaded(LeaderboardScoreUploaded_t result, bool bIOFailure)
    {
        if (bIOFailure || result.m_bSuccess == 0)
        {
            Debug.LogWarning("Score not updated");
        }
        else
        {
        }
        DownloadLeaderboard();
    }

    void DownloadLeaderboard()
    {
        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(
            leaderboard,
            ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends,
            0,
            0
        );
        downloadResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnScoresDownloaded);
        downloadResult.Set(handle);
    }

    void OnScoresDownloaded(LeaderboardScoresDownloaded_t result, bool bIOFailure)
    {
        if (bIOFailure)
        {
            Debug.LogError("Failed to download leaderboard");
            return;
        }

        int entryCount = result.m_cEntryCount;
        leaderboardText = $@"Leaderboard - Readout Start
-----------------------------------

Pilot VNT Records:

";
        int i = 0;
        while (i < entryCount && i < 10)
        {
            LeaderboardEntry_t entry;
            SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, i, out entry, null, 0);
            string friendName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
            if (friendName.Length > 10)
            {
                friendName = friendName.Substring(0, 10);
            }
            else
            {
                friendName = friendName.PadRight(10);
            }
            string textEntry = $"{friendName} : {entry.m_nScore}";
            leaderboardText = leaderboardText + textEntry + "\n";
            i++;
        }
        while (i < 10)
        {
            leaderboardText = leaderboardText + "\n";
            i++;
        }

        leaderboardText += $@"
        

        

-----------------------------------
Readout End";
    }
}
