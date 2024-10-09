using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// using UnityEditor.Build.Content;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;

public class FollowingObject
{
    public string Name {get; private set;}
    public GameObject Object {get; private set;}

    public FollowingObject( string name, GameObject obj)
    {
        Name = name;
        Object = obj;
    }
}

public class GameController : MonoBehaviour
{
    public GameObject deathObject;
    public GlobalVariables globalVariables;
    public static Color ColorForPurple => Instance.globalVariables.colorForPurple;
    public static Color ColorForGreen => Instance.globalVariables.colorForGreen;
    public static float GlobalSkinWidth => Instance.globalVariables.globalSkinWidth;
    public static GameController Instance { get; private set; }
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    [SerializeField] private float waitTimeAfterDamage;
    [SerializeField] private Animator levelChangerAnimator;
    [SerializeField] private PointCounter pointCounter;
    private int score;
    public int Score
    {
        get { return score; }
    }

    private bool timerMoving;
    public bool TimerMoving
    {
        get { return timerMoving; }
    }
    private float bonusTimer = 0;
    public float BonusTimer
    {
        get { return bonusTimer; }
    }


    [SerializeField] private int  timerStartValue = 300;
    private int timer;
    public int Timer
    {
        get { return timer; }
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
        get { return collectedObjects; }
    }

    private Dictionary<string, FollowingObject> followingObjects = new Dictionary<string, FollowingObject>();
    public Dictionary<string, FollowingObject> FollowingObjects
    {
        get { return followingObjects; }
    }

    // private List<FollowingObject> followingObjects = new List<FollowingObject>();
    // public List<FollowingObject> FollowingObjects
    // {
    //     get { return followingObjects; }
    // }

    // ----------------------------------------------------------------------------------

    // ---------------------------------------------------------------------------------- 
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            collectedObjects = SaveDataManager.LoadGameData().collectedObjects ?? new HashSet<string>();
            SceneManager.sceneLoaded += OnSceneLoad;

            if (player != null) {
                StartTimer(timerStartValue);
            }
        }
        else
        {
            if (player != null && !Instance.TimerMoving) {
                Instance.StartTimer(timerStartValue);
            }
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
            UpdateHighScores();
            Application.Quit();
#endif
        }

        if (bonusTimer > 0 )
        {
            if (timerMoving)
            {
                bonusTimer -= Time.deltaTime;
            }
        }
        else if(timerMoving)
        {
            bonusTimer = 0;
            timerMoving = false;
            if (deathObject)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject _deathObject = Instantiate(deathObject, player.transform.position, Quaternion.identity);
                _deathObject.transform.SetParent(null);
                // print(player);
                _deathObject.GetComponentInChildren<DeathScript>().SetPlayer(player);
                player.SetActive(false);

            }
            else
            {
                ResetGameState();
                ChangeScene("Main Menu");
            }
        }
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        levelChangerAnimator.Play("Fade_In", 0, 0f);
        objectPool = new Dictionary<string, Queue<GameObject>>();

        int i = 1;
        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value != null)
            {
                FollowPlayer followPlayer = entry.Value.Object.GetComponent<FollowPlayer>();
                if (followPlayer)
                {
                    followPlayer.offset = i;
                    followPlayer.AssignToPlayer();
                    i++;
                }
                else
                {
                    RemoveFollowingObject(entry.Key);
                }
            }
        }
    }

    // ------------------------------
    // Events
    // ------------------------------
    public event Action OnRoomStateChanged;
    public event Action OnPlayerDashed;
    public event Action OnUpdateHUD;
    public event Action OnPlayerDamaged;

    public RoomColor ToggleRoomState()
    {
        roomState = roomState == RoomColor.Purple ? RoomColor.Green : RoomColor.Purple;
        OnRoomStateChanged?.Invoke();
        return roomState;
    }

    public void InvokePlayerDashed()
    {
        OnPlayerDashed?.Invoke();
    }


    // ------------------------------
    // Update Variables in Controller
    // ------------------------------
    public void ResetGameState()
    {
        bonusTimer = 0;
        timerMoving = false;

        health = 5;
        UpdateHighScores();
        score = 0;
        collectedObjects.Clear();

        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.score = score;
        gameData.collectedObjects = collectedObjects;
        SaveDataManager.SaveGameData(gameData);

        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value.Object)
            {
                Destroy(entry.Value.Object);
            }
        }
        followingObjects.Clear();
    }

    private void UpdateHighScores() {
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Add(score);
        gameData.highScores.Sort((a,b) => b.CompareTo(a));
        while (gameData.highScores.Count > 10) {
            gameData.highScores.RemoveAt(gameData.highScores.Count - 1);
        }
        SaveDataManager.SaveGameData(gameData);
    }

    public int AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        OnUpdateHUD?.Invoke();
        return score;
    }
    public int ChangeHealth(int healthChange, bool shouldResetRoom = false)
    {
        if (health <= 0) return 0;
        if (healthChange < 0) OnPlayerDamaged?.Invoke();
        // health += healthChange;
        ChangeCharge(0);
        health = Mathf.Clamp(health, 0, maxHealth);
        OnUpdateHUD?.Invoke();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player playerScript = player.GetComponent<Player>();
        if (health <= 0)
        {
            //Reset Game State
            if (deathObject)
            {
                // GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject _deathObject = Instantiate(deathObject, player.transform.position, Quaternion.identity);
                _deathObject.transform.SetParent(null);
                // print(player);
                _deathObject.GetComponentInChildren<DeathScript>().SetPlayer(player);
                player.SetActive(false);

            }
            else
            {
                ResetGameState();
                ChangeScene("Main Menu");
            }

        }
        else if (healthChange < 0)
        {
            foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
            {
                if (entry.Value.Object)
                {
                    Destroy(entry.Value.Object);
                }
            }
            followingObjects.Clear();
            if (shouldResetRoom)
            {
                StartCoroutine(WaitAndChangeScene(waitTimeAfterDamage));

            }
            else
            {
                playerScript.HideAndStartRespawn();
            }
        }
        return health;
    }

    public void StartTimer(float timerValue)
    {
        timerMoving = true;
        bonusTimer = timerValue;
    }
    public void ResumeTimer()
    {
        timerMoving = true;
    }
    public void AddToTimer(int timeToAdd)
    {
        bonusTimer += timeToAdd;
        if (bonusTimer > 3599) 
        {
            bonusTimer = 3599;
        }
    }
    public void StopTimer()
    {
        int pointsToAdd = 100 * (int) bonusTimer;
        print(bonusTimer + " - " + pointsToAdd);
        AddToScore(pointsToAdd);
        timerMoving = false;
    }

    public IEnumerator WaitAndReactivatePlayer(Player player, float timeToWait)
    {

        yield return new WaitForSeconds(timeToWait);
        if (player)
        {
            player.Respawn();
        }

    }

    IEnumerator WaitAndChangeScene(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public int ChangeCharge(int chargeChange)
    {
        charge += chargeChange;
        if (charge >= ChargeMax)
        {
            if (health < maxHealth)
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

    public void AddFollowingObjects(string key, string name, GameObject gameObject)
    {
        if (followingObjects.ContainsKey(key)) return;
        FollowingObject followingObject = new FollowingObject(name,gameObject);
        followingObjects.Add(key, followingObject);
        int i = 1;
        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value != null)
            {
                FollowPlayer followPlayer = entry.Value.Object.GetComponent<FollowPlayer>();
                if (followPlayer)
                {
                    followPlayer.offset = i;
                    i++;
                }
            }
        }
    }
    public void RemoveFollowingObject(string objectKey)
    {
        if (!followingObjects.ContainsKey(objectKey)) return;
        followingObjects.TryGetValue(objectKey, out FollowingObject obj);
        if (obj != null)
        {
            Destroy(obj.Object);
        }
        followingObjects.Remove(objectKey);
    }


    // ---------------------------------
    // Functions other objects can Call
    // ---------------------------------
    public GameObject PullFromPool(GameObject gameObject, Vector3 position)
    {
        //Name of the specific pool to access
        string tag = gameObject.name;

        //If there is no pool with that name create one and add the newly instanciated object to it
        if (!objectPool.ContainsKey(tag))
        {
            objectPool.Add(tag, new Queue<GameObject>());
            GameObject newObject = Instantiate(gameObject, position, Quaternion.identity);
            objectPool[tag].Enqueue(newObject);
            return newObject;
        }

        //if the next object in the pool is already active, create a new one and add it to the queue
        if (objectPool[tag].Peek().activeSelf)
        {
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

    public void ChangeScene(string sceneName)
    {
        levelChangerAnimator.SetTrigger("FadeIn");
        StartCoroutine(ChangeSceneAfterDelay(sceneName));
    }
    public IEnumerator ChangeSceneAfterDelay(string sceneName)
    {
        yield return null;
        AnimatorStateInfo stateInfo = levelChangerAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        SceneManager.LoadScene(sceneName);
    }
    public void SaveGame()
    {

        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.score = score;
        gameData.collectedObjects = collectedObjects;
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

    public void ShowPointCounter(int pointsToAdd, Vector3 position, bool isCombo = true)
    {
        if (!pointCounter) return;
        pointCounter.transform.position = position;
        pointCounter.AddPointsToTotal(pointsToAdd, isCombo);
    }

    public void EndPointCombo()
    {
        pointCounter.EndCombo();
    }
}
