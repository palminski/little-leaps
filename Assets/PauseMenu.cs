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
        Camera camera = Camera.main;
        if (camera != null)
        {
            transform.position = new(camera.transform.position.x, camera.transform.position.y, 0);
        }

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

    void OnOpenMenu()
    {
        Continue();
    }


    public void ReturnToMainMenu()
    {
        // GameController.Instance.AddToTimer(-1000000000);
        // GameController.Instance.ChangeHealth(-776);
        GameController.Instance.ChangeHealth(-500);
        // Destroy(gameObject);
        // if (playerInput) playerInput.enabled = false;
        
        // if (GameController.Instance.TimerMoving)
        // {
        //     GameController.Instance.StopTimer();
        // }

        // SaveData gameData = SaveDataManager.LoadGameData();
        // gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        // float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;
        // if (GameController.Instance.Score > lowestHighestScore)
        // {
        //     GameController.Instance.ChangeScene("New High Score Menu");
        // }
        // else
        // {
        //     GameController.Instance.ChangeScene("Game Over Menu");
        // }
    }

    public void Continue()
    {
        Destroy(gameObject);
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
