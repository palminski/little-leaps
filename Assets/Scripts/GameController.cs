using UnityEditor.Build.Content;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public static GameController Instance {get; private set;}

    private int score ;
    public int Score {
        get {return score;}
    }

    private int roomState = 0;
    public int RoomState {
        get {return roomState;}
    }


    public int AddToScore(int pointsToAdd) {
        score += pointsToAdd;
        return score;
    }

    public int ToggleRoomState() {
        roomState = (roomState == 0 ? 1 : 0);
        return roomState;
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

}
