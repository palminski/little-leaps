using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private Animator levelChangerAnimator;
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
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        // print("HELLO WORLD");
        // levelChangerAnimator.SetTrigger("FadeOut");
        levelChangerAnimator.Play("Fade_In", 0, 0f);
        objectPool =  new Dictionary<string, Queue<GameObject>>();
    }

    

    public event Action OnRoomStateChanged;
    public event Action OnUpdateHUD;

    public int AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        OnUpdateHUD?.Invoke();
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
        OnUpdateHUD?.Invoke();
        if (health <= 0) {
            health = 5;
            // SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            ChangeScene("Main Menu");
        } 
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

    public void ChangeScene(string sceneName) {
        levelChangerAnimator.SetTrigger("FadeIn");
        StartCoroutine(ChangeSceneAfterDelay(sceneName));
    }
    public IEnumerator ChangeSceneAfterDelay(string sceneName) {
        yield return null;
        AnimatorStateInfo stateInfo = levelChangerAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        SceneManager.LoadScene(sceneName);
    }
}
