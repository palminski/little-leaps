using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActions : MonoBehaviour
{
    public void Test() 
    {
        Debug.Log("TEST");
    }

    public void AddToScore(int points)
    {
        GameController.Instance.AddToScore(points);
    }
}
