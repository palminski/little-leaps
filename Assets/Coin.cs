using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    [SerializeField]
    private int pointValue = 100;

    [SerializeField]
    private int chargeValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        print("test");
        if (hitCollider.gameObject == player)
        {
            GameController.Instance.AddToScore(pointValue);
            GameController.Instance.ChangeCharge(chargeValue);
            Destroy(gameObject);
        }
    }
}
