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
        newColor.a -= fadeSpeed * Time.deltaTime;



        if (newColor.a <= 0)
        {
            SaveData gameData = SaveDataManager.LoadGameData();
            gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
            float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;

            if (GameController.Instance.Score > lowestHighestScore)
            {
                GameController.Instance.ChangeScene("New High Score Menu");
            }
            else
            {
                GameController.Instance.ChangeScene("Game Over Menu");
            }
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
