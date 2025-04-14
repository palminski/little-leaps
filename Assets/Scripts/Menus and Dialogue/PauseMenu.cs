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

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        Time.timeScale = 0;

        // playerInput = GetComponent<PlayerInput>();
        selectedIndex = 0;
        currentOptions = menuOptions;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>())
        {
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Menu");
        }
        else
        {
            Continue();
        }
        UpdateMenuText();
    }

    void Update()
    {
        if (cam != null)
        {
            transform.position = new(cam.transform.position.x, cam.transform.position.y, 0);
        }

        //Inputs from player
        if (playerInput)
        {
            var inputActions = playerInput.actions;

            if (inputActions["NavigateRight"].WasPressedThisFrame())
            {
                OnNavigateRight();
            }
            if (inputActions["NavigateLeft"].WasPressedThisFrame())
            {
                OnNavigateLeft();
            }
            if (inputActions["NavigateDown"].WasPressedThisFrame())
            {
                OnNavigateDown();
            }
            if (inputActions["NavigateUp"].WasPressedThisFrame())
            {
                OnNavigateUp();
            }
            if (inputActions["Select"].WasPressedThisFrame())
            {
                OnSelect();
            }

            if (inputActions["Back"].WasPressedThisFrame())
            {
                OnCancel();
            }
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
        if (playerInput != null) playerInput.SwitchCurrentActionMap("Movement");
    }

    void OnOpenMenu()
    {
        Continue();
    }


    public void KillPlayer()
    {
        Time.timeScale = 1;

        
        GameController.Instance.ChangeHealth(-776);
        // SaveData gameData = SaveDataManager.LoadGameData();
        // gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        // float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;

        // if (GameController.Instance.Score > lowestHighestScore)
        // {
        //     GameController.Instance.ChangeScene("New High Score Menu");
        // }
        // else
        // {
        //     GameController.Instance.ChangeScene("Main Menu");
        // }

    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;

        GameController.Instance.SetShouldSkipGameover(true);
        GameController.Instance.ChangeHealth(-776);
        // SaveData gameData = SaveDataManager.LoadGameData();
        // gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        // float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;

        // if (GameController.Instance.Score > lowestHighestScore)
        // {
        //     GameController.Instance.ChangeScene("New High Score Menu");
        // }
        // else
        // {
        //     GameController.Instance.ChangeScene("Main Menu");
        // }

    }

    public void Continue()
    {
        Destroy(gameObject);

    }

    public void ToggleSFX()
    {
        if (AudioController.Instance != null) AudioController.Instance.ToggleSFX();

    }

    public void ToggleMusic()
    {
        if (AudioController.Instance != null) AudioController.Instance.ToggleMusic();

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

    void OnCancel()
    {
        Continue();
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
