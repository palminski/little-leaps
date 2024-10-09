using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoomAfterDialogue : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName;
    // Start is called before the first frame update
    public void ChangeRoom()
    {
        GameController.Instance.ChangeScene(targetSceneName);
    }
}
