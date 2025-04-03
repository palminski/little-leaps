using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoomAfterDialogue : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName;

    [SerializeField] private bool isLastScene = false;
    // Start is called before the first frame update
    public void ChangeRoom()
    {
        if (isLastScene)
        {
            if (AudioController.Instance != null) AudioController.Instance.StartMusicFade();
            SaveData gameData = SaveDataManager.LoadGameData();
            gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
            float lowestHighestScore = gameData.highScores.Count > 0 ? gameData.highScores[gameData.highScores.Count - 1].score : 0;

            GameController.Instance.SetShouldSkipGameover(true);
            if (GameController.Instance.Score > lowestHighestScore)
            {
                GameController.Instance.ChangeScene("New High Score Menu");
            }
            else
            {
                GameController.Instance.ChangeScene("Main Menu");
            }
        }
        
        GameController.Instance.ChangeScene(targetSceneName, true);
    }
    
}
