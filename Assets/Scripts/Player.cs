using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{

    

    private void Awake() {
        
    }

    void OnJump() {
        Debug.Log("Jump");
    }

    void OnMove(InputValue value) {
        float moveValue = value.Get<float>();
        Debug.Log(moveValue);
    }
}
