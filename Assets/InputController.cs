using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

using System;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    private string lastUsedDevice = "Keyboard";

    public event Action OnFinishedRebind;
    public event Action OnRestoredToDefault;


    // public PlayerInput playerInput;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Awake()
    {
        // if (playerInput == null) playerInput = FindObjectOfType<PlayerInput>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // LoadBindings();
        
    }

    //Check Player Prefs For Bindings and Apply Them As Needed
    public void LoadBindings()
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var input in playerInputs)
        {
            LoadBinding(input);
        }
    }

    public void LoadBinding(PlayerInput input)
    {
        // foreach (var action in input.actions)
        // {
        //     print(action.name);
        //     if (action.name != "ToggleRoom") continue;
        //     print("FOUND!!!!");
        //     print(PlayerPrefs.GetString(action.name + "_binding", ""));
        //     string bindingJson = PlayerPrefs.GetString(action.name + "_binding", "");
        //     if (!string.IsNullOrEmpty(bindingJson))
        //     {
        //         print(action.name);
        //         action.LoadBindingOverridesFromJson(bindingJson, true);
        //     }
        // }
        SaveData savedata = SaveDataManager.LoadGameData();
        foreach (var action in input.actions)
        {
            action.RemoveAllBindingOverrides();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                string controlScheme = action.bindings[i].path.Contains("Gamepad") ? "gamepad" : "keyboard";
                string key = $"{action.name}_binding_{controlScheme}_{i}";
                // string savedBinding = PlayerPrefs.GetString(key, "");
                ControlBinding savedBinding = savedata.bindings.Find(binding => binding.key == key);

                if (savedBinding != null)
                {
                    // print(savedBinding);
                    action.ApplyBindingOverride(i, savedBinding.binding);
                }
            }
        }
    }

    public void SaveBinding(InputAction action)
    {
        // string bindingJson = action.SaveBindingOverridesAsJson();
        // print(bindingJson);
        // PlayerPrefs.SetString(action.name+"_binding", bindingJson);
        // PlayerPrefs.Save();

        for (int i = 0; i < action.bindings.Count; i++)
        {
            string controlScheme = action.bindings[i].path.Contains("Gamepad") ? "gamepad" : "keyboard";
            string key = $"{action.name}_binding_{controlScheme}_{i}";

            SaveData saveData = SaveDataManager.LoadGameData();
            saveData.bindings.RemoveAll(binding => binding.key == key);
            ControlBinding newBinding = new ControlBinding { key = key, binding = action.bindings[i].effectivePath };
            saveData.bindings.Add(newBinding);
            SaveDataManager.SaveGameData(saveData);
            // PlayerPrefs.SetString(key, action.bindings[i].effectivePath);


        }
        // PlayerPrefs.Save();
    }


    public void StartRebinding(string actionName, string controlScheme, System.Action<string> onComplete)
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        // print(playerInput);
        // print(playerInput.currentControlScheme);
        // return;
        // string lastUsedDevice = playerInput.currentControlScheme.Contains("Gamepad") ? "Gamepad" : "Keybaord";
        if (playerInput == null) return;
        var action = playerInput.actions.FindAction(actionName);
        if (action == null)
        {
            return;
        }

        int bindingIndex = -1;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (controlScheme == "Keyboard" && action.bindings[i].path.Contains("Keyboard"))
            {
                bindingIndex = i;
                break;
            }
            else if (controlScheme == "Gamepad" && action.bindings[i].path.Contains("Gamepad"))
            {
                bindingIndex = i;
                break;
            }
        }

        if (bindingIndex == -1)
        {
            return;
        }

        action.Disable();


        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
        .WithControlsExcluding(controlScheme == "Gamepad" ? "Keyboard" : "Gamepad")
        .WithControlsExcluding("Mouse")
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation =>
        {
            action.Enable();
            SaveBinding(action);
            onComplete?.Invoke(action.bindings[bindingIndex].ToDisplayString());
            rebindingOperation.Dispose();
            OnFinishedRebind?.Invoke();
        })
        .Start();
    }

    public void ResetRebindings(string lastUsedSCheme)
    {
        string controlScheme = lastUsedSCheme == "Gamepad" ? "gamepad" : "keyboard";

        SaveData saveData = SaveDataManager.LoadGameData();
        saveData.bindings.RemoveAll(binding => binding.key.Contains(controlScheme));
        SaveDataManager.SaveGameData(saveData);


        OnRestoredToDefault?.Invoke();

        // print(lastUsedSCheme);
    }

    public void SetLastUsedDevice(string device)
    {
        if(device == null) return;
        lastUsedDevice = device;
    }
    public string GetLastUsedDevice()
    {
        return lastUsedDevice;
    }


}
