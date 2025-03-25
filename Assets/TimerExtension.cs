using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TimerExtension : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string id = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id))
        {
            Destroy(gameObject);
            return;
        }
        if (GlobalConstants.prestigeTime.ContainsKey(GameController.Instance.SessionPrestige))
        {
            float timeSet = GlobalConstants.prestigeTime[GameController.Instance.SessionPrestige];
            if (timeSet > GameController.Instance.BonusTimer)
            {
                GameController.Instance.StartTimer(MathF.Max(90, timeSet));
                GameController.Instance.SetStartingTimer(MathF.Max(90, timeSet));
            }
        }
        else
        {
            GameController.Instance.StartTimer(690f);
            GameController.Instance.SetStartingTimer(690f);
        }
        GameController.Instance.TagObjectStringAsCollected(id);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
