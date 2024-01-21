using System;
// using UnityEditor.Build.Content;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GlobalVariables globalVariables;

    public static Color ColorForPurple => Instance.globalVariables.colorForPurple;
    public static Color ColorForGreen => Instance.globalVariables.colorForGreen;

    public static float GlobalSkinWidth => Instance.globalVariables.globalSkinWidth;
    
    
    public static GameController Instance { get; private set; }

    private int score;
    public int Score
    {
        get { return score; }
    }

    private RoomColor roomState = RoomColor.Purple;
    public RoomColor RoomState
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

    public RoomColor ToggleRoomState()
    {
        roomState = roomState == RoomColor.Purple ? RoomColor.Green : RoomColor.Purple;
        OnRoomStateChanged?.Invoke();
        return roomState;
    }

    public int ChangeHealth(int healthChange) {
        health += healthChange;
        return health;
    }

}
