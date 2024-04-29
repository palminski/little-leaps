using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackTrigger : MonoBehaviour
{
    private InputAction joystickTilt;
    private PlayerInput playerInput;

    private Player player;
    public GameObject attackObject;
    private Animator attackAnimator;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        attackAnimator = attackObject.GetComponent<Animator>();
        
        joystickTilt = playerInput.actions["RightJoystickTilt"];
    }

    private void OnEnable()
    {
        joystickTilt.Enable();
        joystickTilt.started += TiltAttack;
    }

     private void OnDisable()
    {
        joystickTilt.Disable();
        joystickTilt.started -= TiltAttack;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 joystickVector = joystickTilt.ReadValue<Vector2>();
        // if (joystickVector != Vector2.zero)
        // {
        //     float angle = Mathf.Atan2(joystickVector.y, joystickVector.x) * Mathf.Rad2Deg;
        //     print("JOYSTICK ANGLE "+ angle);
        // }
    }

    private void TiltAttack(InputAction.CallbackContext context) {
        Vector2 joystickVector = context.ReadValue<Vector2>();
        if (joystickVector != Vector2.zero)
        {
            
            float angle = Mathf.Atan2(joystickVector.y, joystickVector.x) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 90) * 90;
            
            LaunchAttack(angle);
        }
    }

    private void OnAttack()
    {
        Vector2 leftJoystickPosition = playerInput.actions["LeftJoystickTilt"].ReadValue<Vector2>();

        float angle = 0;
        if (leftJoystickPosition != Vector2.zero){
            angle = Mathf.Atan2(leftJoystickPosition.y, leftJoystickPosition.x) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 90) * 90;
        }
        else if(player.transform.localScale.x == -1)
        {
            angle = 180;
        }
        
        LaunchAttack(angle);
    }

    private void LaunchAttack(float angle)
    {
        if (attackObject.activeSelf) return;
        Transform attackObjectTransform = attackObject.transform;
        attackObjectTransform.rotation = Quaternion.Euler(0,0,angle);
        attackObject.SetActive(true);
        attackAnimator.SetTrigger("Attack");
    }
}
