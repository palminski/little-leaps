using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Random.value > 0.5f) GameController.Instance.SetRoomState(RoomColor.Green);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(8f);
        GameController.Instance.ChangeScene("Main Menu");
    }
}
