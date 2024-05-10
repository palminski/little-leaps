using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class MenuOption
{
    public TMP_Text textElement;
    public UnityEvent action;

    public string text;

}

public class MainMenu : MonoBehaviour
{
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] menuOptions;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        UpdateMenuText();
    }

    public void StartGame()
    {
        // SaveData saveData = SaveDataManager.LoadGameData();
        // GameController.Instance.AddToScore(saveData.score);
        LevelConnection.ActiveConnection = null;
        GameController.Instance.ChangeScene("Main Room");
    }
    public void Debug()
    {
        print("Debug");
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
        menuOptions[selectedIndex].action.Invoke();
    }

    private void UpdateIndex(int ammount)
    {
        int maxIndex = menuOptions.Length - 1;
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

    private void UpdateMenuText() {
        foreach (MenuOption menuOption in menuOptions) {
            menuOption.textElement.text = menuOption.text;
            if (menuOptions[selectedIndex] == menuOption) {
                menuOption.textElement.text = "> " + menuOption.text;
            }
            
        }
    }


}
