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
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
     public TMP_Text currentText;
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] currentOptions;
    public MenuOption[] menuOptions;

    public GameOverText startingText;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        currentOptions = menuOptions;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        
        UpdateMenuText();

    }

    public void ReturnToMainMenu()
    {
        GameController.Instance.ChangeScene("Main Menu");
    }

    public void Continue()
    {
        if (GameController.Instance.Checkpoint !=null && GameController.Instance.Checkpoint != "" && DoesSceneExists(GameController.Instance.Checkpoint))
        {
            LevelConnection.ActiveConnection = null;
            GameController.Instance.ChangeScene(GameController.Instance.Checkpoint);
            GameController.Instance.SetCheckPoint("Main Menu");
        }
        else
        {
            LevelConnection.ActiveConnection = null;
            GameController.Instance.ChangeScene("lv_1_start");
        }
        
    }

    // ==============================================================================
    // Menu Navigation
    // ==============================================================================
    void OnNavigateRight()
    {
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateLeft()
    {
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateDown()
    {
        UpdateIndex(1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnNavigateUp()
    {
        UpdateIndex(-1);
        if (AudioController.Instance != null) AudioController.Instance.PlayMoveCursor();
    }

    void OnSelect()
    {
        currentOptions[selectedIndex].action.Invoke();
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();
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
            textLine += "\n";;
            currentText.text += textLine;
        }
        if (startingText.isFinishedTyping) startingText.SetSideText(currentOptions[selectedIndex].sideText);
    }

    private bool DoesSceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    
}
