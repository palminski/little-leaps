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
        if (GameController.Instance.Checkpoint !=null && GameController.Instance.Checkpoint != "")
        {
            GameController.Instance.ChangeScene(GameController.Instance.Checkpoint);
        }
        else
        {
            GameController.Instance.ChangeScene("Main Room");
        }
        
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

    

    
}
