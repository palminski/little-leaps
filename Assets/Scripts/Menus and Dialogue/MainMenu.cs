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
    public string requiredKey;
    public string text;

    [TextArea(15, 15)] public string sideText;

}

public class MainMenu : MonoBehaviour
{
    public TMP_Text currentText;
    public TMP_Text mainMenu;
    public TMP_Text levelSelectMenu;
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] currentOptions;
    public MenuOption[] menuOptions;
    public MenuOption[] levelSelectOptions;
    public StartingText startingText;
    private List<string> permCollected;
    public Dictionary<string, MenuOption> levelDictionary;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        gameObject.SetActive(false);

        permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;
        levelSelectOptions = levelSelectOptions.Where(option => option.requiredKey.Length == 0 || permCollected.Contains(option.requiredKey)).ToArray();
    }

    void OnEnable()
    {
        SetCurrentMenu(menuOptions,mainMenu);
        UpdateMenuText();

    }

    public void StartGame()
    {
        SetCurrentMenu(levelSelectOptions, levelSelectMenu);
        // LevelConnection.ActiveConnection = null;
        // GameController.Instance.ResetGameState();
        // GameController.Instance.ChangeScene("sector_0_start");
    }

    public void StartLevel(string levelToStart)
    {
        LevelConnection.ActiveConnection = null;
        GameController.Instance.ResetGameState();
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
        SetCurrentMenu(menuOptions,mainMenu);
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
                textLine += "> ";
            }
            else
            {
                textLine += "  ";
            }
            textLine += menuOption.text + "\n";

            currentText.text += textLine;
        }
        if (startingText.isFinishedTyping) startingText.SetSideText(currentOptions[selectedIndex].sideText);
    }

    private void SetCurrentMenu(MenuOption[] newMenuOptions, TMP_Text newMenuText)
    {
        DisableMenus();
        if (newMenuOptions == menuOptions)
        {
            startingText.gameObject.SetActive(true);
        }
        newMenuText.gameObject.SetActive(true);
        currentText = newMenuText;
        currentOptions = newMenuOptions;
        selectedIndex = 0;
        UpdateMenuText();
    }

    private void DisableMenus()
    {
        startingText.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        levelSelectMenu.gameObject.SetActive(false);
    }


}
