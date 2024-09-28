using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int score;
    public int health;
    public int lives;

    public List<int> highScores = new List<int>{0,10,0,0,100,0,0,0,0,0};

    public HashSet<string> collectedObjects;

    public string currentScene;
}