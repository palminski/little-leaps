using System;
using System.Collections.Generic;

[Serializable]
public class HighScore
{
    public string name;
    public int score;
    public HighScore(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

[Serializable]
public class BestTime
{
    public string level;
    public float time;
    public BestTime(string level, float time)
    {
        this.level = level;
        this.time = time;
    }
}

[Serializable]
public class BestScore
{
    public string level;
    public float score;
    public BestScore(string level, float score)
    {
        this.level = level;
        this.score = score;
    }
}


[Serializable]
public class SaveData
{
    // Temporary Saved Data
    public int score;
    public int health;
    public int lives;
    public HashSet<string> collectedObjects;
    public string currentScene;
    
    // Permanent Saved Data
    public List<HighScore> highScores = new List<HighScore>{
        new("TST", 100),
        new("TST", 100),
        new("TST", 200),
        new("TST", 100),
        new("TST", 100),
        new("TST", 300),
        new("TST", 100),
        new("TST", 100),
        new("TST", 500),
        new("TST", 100),
    };

    public List<BestTime> bestTimes = new List<BestTime>();
    public List<BestScore> bestScores = new List<BestScore>();
    public List<string> permanentCollectedObjects = new List<string>();

    public int prestige = 0;
    public int maxLives = 8;
    public float healing = 0.05f;

    
}