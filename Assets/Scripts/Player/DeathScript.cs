using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScript : MonoBehaviour
{
    private Image playerDeathImage;
    private GameObject player;
    public bool shouldFade = false;
    private readonly float fadeSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        playerDeathImage = GetComponent<Image>();
        Camera.main.GetComponent<CameraControls>().canMove = false;

        if (GameController.Instance.RoomState == RoomColor.Purple)
        {
            playerDeathImage.color = GameController.ColorForPurple;
        }
        else
        {
            playerDeathImage.color = GameController.ColorForGreen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color newColor = playerDeathImage.color;

        if (true)
        {
            newColor.a -= fadeSpeed * Time.deltaTime;
        }




        if (newColor.a <= 0)
        {
            
            Destroy(gameObject);
        }
        playerDeathImage.color = newColor;
    }

    public void SetPlayer(GameObject player)
    {

        transform.localScale = player.transform.localScale;
        transform.localScale = player.transform.localScale;

    }
}
