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
public class BestChips
{
    public string level;
    public int chips;
    public BestChips(string level, int chips)
    {
        this.level = level;
        this.chips = chips;
    }
}

[Serializable]
public class ControlBinding
{
    public string key;
    public string binding;
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
        
        new("LIF", 77600),
        new("MIM", 70000),
        new("WKA", 60000),
        new("WIL", 50000),
        new("KAT", 40000),
        new("BEN", 25000),
        new("SMN", 10000),
        new("KEV", 7500),
        new("KAI", 5000),
        new("DRW", 1000),
    };

    public List<BestTime> bestTimes = new List<BestTime>();
    public List<BestScore> bestScores = new List<BestScore>();
    public List<BestChips> bestChips = new List<BestChips>();
    public List<ControlBinding> bindings = new List<ControlBinding>();
    public List<string> permanentCollectedObjects = new List<string>();

    // public int prestige = 0;
    // public int maxLives = 8;
    // public float healing = 0.05f;

    
}