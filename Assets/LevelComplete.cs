using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public string levelName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteLevel()
    {
        // print("here");
        // if (GameController.Instance.TimerMoving)
        // {
        //     GameController.Instance.StopTimer();
        // }
        // if (levelName.Length > 0)
        // {
        //     float levelTime = GameController.Instance.BonusTimer;
        //     if (levelTime <= 0) return;
        //     SaveDataManager.AddPermanentCollectedString(levelName);
        //     SaveData saveData = SaveDataManager.LoadGameData();
        //     BestTime existingTime = saveData.bestTimes.FirstOrDefault(time => time.level == levelName);
        //     if (existingTime != null)
        //     {
        //         if(levelTime < existingTime.time)
        //         {
        //         existingTime.time = levelTime;
        //         SaveDataManager.SaveGameData(saveData);    
        //         }
                
        //     }
        //     else
        //     {
        //         saveData.bestTimes.Add(new BestTime(levelName, levelTime));
        //         SaveDataManager.SaveGameData(saveData);    
        //     }
        // }
    }
}
