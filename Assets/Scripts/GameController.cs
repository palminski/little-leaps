using System;
using System.Collections.Generic;

// using UnityEditor.Build.Content;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public GlobalVariables globalVariables;

    public static Color ColorForPurple => Instance.globalVariables.colorForPurple;
    public static Color ColorForGreen => Instance.globalVariables.colorForGreen;

    public static float GlobalSkinWidth => Instance.globalVariables.globalSkinWidth;
    
    
    public static GameController Instance { get; private set; }

    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

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

    public GameObject PullFromPool(GameObject gameObject, Vector3 position) {
        //Name of the specific pool to access
        string tag = gameObject.name;

        //If there is no pool with that name create one and add the newly instanciated object to it
        if (!objectPool.ContainsKey(tag)) {
            objectPool.Add(tag,new Queue<GameObject>());
            GameObject newObject = Instantiate(gameObject, position, Quaternion.identity);
            objectPool[tag].Enqueue(newObject);
            return newObject;
        }

        //if the next object in the pool is already active, create a new one and add it to the queue
        if (objectPool[tag].Peek().activeSelf) {
            GameObject newObject = Instantiate(gameObject, position, Quaternion.identity);
            objectPool[tag].Enqueue(newObject);
            return newObject;
        }

        //Otherwise we will pull an object from the queue, set it to active, and place it in the back of the queue
        GameObject pulledObject = objectPool[tag].Dequeue();
        pulledObject.transform.position = position;
        pulledObject.SetActive(true);
        objectPool[tag].Enqueue(pulledObject);

        return pulledObject;
    } 

}
