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

public class PauseMenu : MonoBehaviour
{
     public TMP_Text currentText;
    private PlayerInput playerInput;
    private int selectedIndex;
    public MenuOption[] currentOptions;
    public MenuOption[] menuOptions;

    public TMP_Text descriptionText;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        currentOptions = menuOptions;
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            playerInput.enabled = false;
        }
        UpdateMenuText();
    }

    void OnDestroy()
    {
        if (playerInput) playerInput.enabled = true;
    }


    public void ReturnToMainMenu()
    {
        GameController.Instance.AddToTimer(-1000000000);
        Destroy(gameObject);
    }

    public void Continue()
    {
        Destroy(gameObject);
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
        if (descriptionText) descriptionText.text = menuOptions[selectedIndex].sideText;
    }
}
