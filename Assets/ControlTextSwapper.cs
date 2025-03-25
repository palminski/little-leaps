using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTextSwapper : MonoBehaviour
{
    [SerializeField] private string controlToDisplay;
    WorldDialogue worldDialogue;
    [SerializeField] PlayerInput playerInput;
    private 
    // Start is called before the first frame update
    void Start()
    {
        worldDialogue = GetComponent<WorldDialogue>();

        if (playerInput == null && GameObject.FindGameObjectWithTag("Player"))
        {
            playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        }

        // PlayerInput playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        if (worldDialogue.textToType.Contains("[BINDING]"))
            {
                // print(playerInput.currentActionMap.name);
                // foreach(var inputAction in playerInput.actions)
                // {
                //     print(inputAction.name);
                // }
                var action = playerInput.actions.FindAction(controlToDisplay);
                
                if (action != null)
                {
                    string currentControlScheme = (InputController.Instance != null) ? InputController.Instance.GetLastUsedDevice() : "";
                    if (!string.IsNullOrEmpty(currentControlScheme))
                    {
                        foreach (var binding in action.bindings)
                        {
                            if (binding.path.Contains(currentControlScheme == "Gamepad" ? "Gamepad" : "Keyboard"))
                            {
                                print(binding.ToDisplayString());
                                worldDialogue.textToType = worldDialogue.textToType.Replace("[BINDING]", "[ " + binding.ToDisplayString() + " ]");
                                break;
                            }
                        }
                    }
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
