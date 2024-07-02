using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int score;
    public int health;
    public int lives;

    public HashSet<string> collectedObjects;

    public string currentScene;
}