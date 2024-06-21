using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetRoomOnContact : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            GameController.Instance.ChangeHealth(-1);
            GameController.Instance.ChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
