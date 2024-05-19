using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScript : MonoBehaviour
{
    private Image playerDeathImage;
    private GameObject player;
    private readonly float fadeSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.localScale = player.transform.localScale;
        transform.localScale = player.transform.localScale;
        Camera.main.GetComponent<CameraControls>().canMove = false;
        playerDeathImage = GetComponent<Image>();

        if (GameController.Instance.RoomState == RoomColor.Purple) {
            playerDeathImage.color = GameController.ColorForPurple;
        }
        else {   
            playerDeathImage.color = GameController.ColorForGreen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color newColor = playerDeathImage.color;
        newColor.a -= fadeSpeed * Time.deltaTime;
        if (newColor.a <= 0) {
             GameController.Instance.ChangeScene("Main Menu");
             Destroy(gameObject);
        }
        playerDeathImage.color = newColor;
    }
}
