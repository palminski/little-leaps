using System;
// using UnityEditor.Build.Content;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GlobalVariables globalVariables;

    public static Color ColorForRoomstate0 => Instance.globalVariables.colorForRoomState0;
    public static Color ColorForRoomstate1 => Instance.globalVariables.colorForRoomState1;

    public static float GlobalSkinWidth => Instance.globalVariables.globalSkinWidth;
    
    
    public static GameController Instance { get; private set; }

    private int score;
    public int Score
    {
        get { return score; }
    }

    private int roomState = 0;
    public int RoomState
    {
        get { return roomState; }
    }

    private int health = 5;
    public int Health
    {
        get { return health; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action OnRoomStateChanged;

    public int AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        return score;
    }

    public int ToggleRoomState()
    {
        roomState = roomState == 0 ? 1 : 0;
        OnRoomStateChanged?.Invoke();
        return roomState;
    }

    public int ChangeHealth(int healthChange) {
        health += healthChange;
        return health;
    }

}
