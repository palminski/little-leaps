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
    [SerializeField] private Animator levelChangerAnimator;
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

    private int maxHealth = 5;
    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private int charge;
    public int Charge
    {
        get { return charge; }
    }

    public int ChargeMax
    {
        get { return 100; }
    }

    private HashSet<string> collectedObjects = new HashSet<string>();
    public HashSet<string> CollectedObjects
    {
        get {return collectedObjects;}
    }

    // ----------------------------------------------------------------------------------

    // ---------------------------------------------------------------------------------- 
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        levelChangerAnimator.Play("Fade_In", 0, 0f);
        objectPool =  new Dictionary<string, Queue<GameObject>>();
    }

    // ------------------------------
    // Events
    // ------------------------------
    public event Action OnRoomStateChanged;
    public event Action OnUpdateHUD;
    
    public RoomColor ToggleRoomState()
    {
        roomState = roomState == RoomColor.Purple ? RoomColor.Green : RoomColor.Purple;
        OnRoomStateChanged?.Invoke();
        return roomState;
    }


    // ------------------------------
    // Update Variables in Controller
    // ------------------------------
    public int AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        OnUpdateHUD?.Invoke();
        return score;
    }
    public int ChangeHealth(int healthChange) {
        health += healthChange;
        ChangeCharge(0);
        health = Mathf.Clamp(health,0,maxHealth);
        OnUpdateHUD?.Invoke();
        if (health <= 0) {
            health = 5;
            // SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            ChangeScene("Main Menu");
        } 
        return health;
    }

    public int ChangeCharge(int chargeChange) {
        charge += chargeChange;
        if (charge >= ChargeMax)
        {
            if(health < maxHealth)
            {
                charge -= ChargeMax;
                ChangeHealth(1);
            }
            else
            {
                charge = ChargeMax;
            }
        }
        OnUpdateHUD?.Invoke();
        return charge;
    }

    public void TagObjectStringAsCollected(string objectKey)
    {
        collectedObjects.Add(objectKey);
    }


    // ---------------------------------
    // Functions other objects can Call
    // ---------------------------------
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
    public void SaveGame() {
        
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.score = score;
        SaveDataManager.SaveGameData(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void TestDialogueSystem() 
    {
        Debug.Log("TEST");
    }
}
