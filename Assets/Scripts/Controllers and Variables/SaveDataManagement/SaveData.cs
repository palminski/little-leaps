using System;
using System.Collections.Generic;

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
    public List<int> highScores = new List<int>{0,10,0,0,100,0,0,0,0,0};
    public List<string> permanentCollectedObjects = new List<string>();

    
}