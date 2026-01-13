
using UnityEngine;
using System.Linq;

public class possiblyHideOnStartBasedOnScore : MonoBehaviour
{
    //Even at higher scores the chance to hide will never go below this (increasing it makes an object more likely to be hidden)
    [Range(0f, 1f)]
    public float minChanceToHide = 0.4f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        HighScore highScore = gameData.highScores.First();
        int score = highScore.score;

        float hideChance = 1f - Mathf.Clamp01(score / 1000000);
        // 0.9 otherwise if 0 if max

        if (Random.value < Mathf.Min(0.9f, minChanceToHide + hideChance))
        {
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
