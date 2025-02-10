using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// using UnityEditor.Build.Content;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting;

public class FollowingObject
{
    public string Name { get; private set; }
    public GameObject Object { get; private set; }

    public FollowingObject(string name, GameObject obj)
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

    // [SerializeField] private bool ShouldStartTimer;
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


    private int timer;
    public int Timer
    {
        get { return timer; }
    }

    private int startingScore = 0;
    public int StartingScore
    {
        get { return startingScore; }
    }

    private float startingTimer = 0;
    public float StartingTimer
    {
        get { return startingTimer; }
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
        get { return 1000; }
    }

    [SerializeField] private string checkpoint;
    public string Checkpoint
    {
        get { return checkpoint; }
    }

    private HashSet<string> collectedObjects = new HashSet<string>();
    public HashSet<string> CollectedObjects
    {
        get { return collectedObjects; }
    }

    private HashSet<string> sessionCollectedObject = new HashSet<string>();
    public HashSet<string> SessionCollectedObjects
    {
        get { return sessionCollectedObject; }
    }

    private int sessionPrestige = 0;
    public int SessionPrestige
    {
        get { return sessionPrestige; }
    }

    private float sessionHealing = 0.05f;
    public float SessionHealing
    {
        get { return sessionHealing; }
    }

    private float sessionMultiplier = 1;
    public float SessionMultiplier
    {
        get { return sessionMultiplier; }
    }

    private Dictionary<string, FollowingObject> followingObjects = new Dictionary<string, FollowingObject>();
    public Dictionary<string, FollowingObject> FollowingObjects
    {
        get { return followingObjects; }
    }

    public GameObject pauseMenuPrefab;

    // private int prestige = 0;

    private List<GameObject> disabledEnemies = new List<GameObject>();

    // private List<FollowingObject> followingObjects = new List<FollowingObject>();
    // public List<FollowingObject> FollowingObjects
    // {
    //     get { return followingObjects; }
    // }

    // ----------------------------------------------------------------------------------

    // ---------------------------------------------------------------------------------- 
    void Awake()
    {
        

        Cursor.visible = false;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        

        if (Instance == null)
        {
            SaveData saveData = SaveDataManager.LoadGameData();
            int savedPrestige = saveData.prestige;
            int savedMaxLives = saveData.maxLives;
            float savedHealing = saveData.healing;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            collectedObjects = SaveDataManager.LoadGameData().collectedObjects ?? new HashSet<string>();
            SceneManager.sceneLoaded += OnSceneLoad;
            sessionPrestige = savedPrestige;
            maxHealth = savedMaxLives;
            sessionHealing = savedHealing;
            sessionMultiplier = GlobalConstants.getMultiplier(saveData);
        }
        else
        {
            // if (ShouldStartTimer && !Instance.TimerMoving) {
            //     Instance.ResetTimer();
            //     Instance.StartTimer();
            // }
            
            
            Destroy(gameObject);
        }
    }
    void Update()
    {


        
        if (timerMoving)
        {
            bonusTimer -= Time.deltaTime;
            if (bonusTimer <= -29)
            {
                //Reset Game State
                if (deathObject)
                {
                    EndGame();
                }
                bonusTimer = 0;
                timerMoving = false;
            }
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (int.TryParse(currentSceneName, out int currentSceneNumber))
            {
                int nextSceneNumber = currentSceneNumber + 1;
                string nextSceneName = nextSceneNumber.ToString();

                if (Application.CanStreamedLevelBeLoaded(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    ChangeScene("0");
                }
            }
            else
            {
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (int.TryParse(currentSceneName, out int currentSceneNumber))
            {
                int nextSceneNumber = currentSceneNumber - 1;
                string nextSceneName = nextSceneNumber.ToString();

                if (Application.CanStreamedLevelBeLoaded(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    ChangeScene("100");
                }
            }
            else
            {
                return;
            }
        }
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        levelChangerAnimator.Play("Fade_In", 0, 0f);
        objectPool = new Dictionary<string, Queue<GameObject>>();
        disabledEnemies.Clear();
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
    public event Action OnUpdateDashIcon;
    public event Action OnUpdateJumpIcon;
    public event Action OnPlayerDamaged;
    public event Action OnEnemyKilled;

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

    public void InvokeEnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }

    public void InvokeUpdateDashIcon()
    {
        OnUpdateDashIcon?.Invoke();
    }

    public void InvokeUpdateJumpIcon()
    {
        OnUpdateJumpIcon?.Invoke();
    }


    // ------------------------------
    // Update Variables in Controller
    // ------------------------------
    public void ResetGameState()
    {
        SaveData saveData = SaveDataManager.LoadGameData();
        int savedPrestige = saveData.prestige;
        int savedMaxLives = saveData.maxLives;
        float savedHealing = saveData.healing;
        sessionPrestige = savedPrestige;
        sessionHealing = savedHealing;
        sessionMultiplier = GlobalConstants.getMultiplier(saveData);
        // print(SessionPrestige);
        maxHealth = savedMaxLives;

        bonusTimer = 0;
        timerMoving = false;

        health = maxHealth;
        score = 0;

        collectedObjects.Clear();
        sessionCollectedObject.Clear();

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

    public void SetCheckPoint(string scene)
    {
        checkpoint = scene;
    }

    public void SetStartingScore(int score)
    {
        startingScore = score;
    }

    public void SetStartingTimer(float time)
    {
        startingTimer = time;
    }

    public void UpdateHighScores(string name = "???")
    {
        SaveData gameData = SaveDataManager.LoadGameData();
        gameData.highScores.Add(new(name, score));
        gameData.highScores.Sort((a, b) => b.score.CompareTo(a.score));
        while (gameData.highScores.Count > 10)
        {
            gameData.highScores.RemoveAt(gameData.highScores.Count - 1);
        }
        SaveDataManager.SaveGameData(gameData);
    }

    public int AddToScore(int pointsToAdd)
    {
        score += (int)(pointsToAdd * sessionMultiplier);
        OnUpdateHUD?.Invoke();
        return score;
    }
    public int IncreasePrestige(int prestigeToAdd)
    {
        sessionPrestige += prestigeToAdd;
        return sessionPrestige;
    }
    public int ChangeHealth(int healthChange, bool shouldResetRoom = false)
    {
        if (health <= 0) return 0;
        if (healthChange < 0) OnPlayerDamaged?.Invoke();
        
        //edge case for only 1 max health
        bool willHealAtOneHealth = false;
        if (maxHealth == 1 && charge >= ChargeMax)
        {
            willHealAtOneHealth = true;
            charge = 0;
        }
        if (!willHealAtOneHealth) health += healthChange;

        ChangeCharge(0);
        health = Mathf.Clamp(health, 0, maxHealth);
        OnUpdateHUD?.Invoke();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player playerScript = player ? player.GetComponent<Player>() : null;

        if (health <= 0)
        {
            //Reset Game State
            if (deathObject)
            {
                EndGame();
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
            else if (playerScript)
            {
                playerScript.HideAndStartRespawn();
            }
        }
        return health;
    }

    public void StartTimer(float timeToStartAt = 0)
    {
        if (timeToStartAt == 0) return;
        timerMoving = true;
        bonusTimer = timeToStartAt;
    }

    public void SetTimer(float timeToStartAt = 0)
    {
        bonusTimer = timeToStartAt;
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
        // int pointsToAdd = 100 * (int)bonusTimer;
        // AddToScore(pointsToAdd);
        bonusTimer = 0;
        timerMoving = false;
    }

    public void ResetTimer()
    {
        int pointsToAdd = 100 * (int)bonusTimer;
        AddToScore(pointsToAdd);
        timerMoving = false;
        bonusTimer = 0;
    }

    private void EndGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.SetActive(true);

        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject _deathObject = Instantiate(deathObject, player.transform.position, Quaternion.identity);
        _deathObject.transform.SetParent(null);


        _deathObject.GetComponentInChildren<DeathScript>().SetPlayer(player);
        player.SetActive(false);
    }

    public IEnumerator WaitAndReactivatePlayer(Player player, float timeToWait)
    {

        yield return new WaitForSeconds(timeToWait);
        if (player)
        {
            player.Respawn();
            GameController.Instance.ReactivateEnemies();
        }

    }

    IEnumerator WaitAndChangeScene(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public int ChangeCharge(int chargeChange)
    {
        print(sessionHealing);
        charge += (int)(chargeChange * sessionHealing);
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

    public void OpenPauseMenu()
    {
        PauseMenu currentMenu = FindObjectOfType<PauseMenu>();
        if (currentMenu)
        {
            Destroy(currentMenu.gameObject);
            return;
        }
        Instantiate(pauseMenuPrefab, transform.position, Quaternion.identity);
    }

    public void TagObjectStringAsCollected(string objectKey)
    {
        collectedObjects.Add(objectKey);
    }

    public void TagObjectStringAsCollectedForSession(string objectKey)
    {
        sessionCollectedObject.Add(objectKey);
    }

    public void AddFollowingObjects(string key, string name, GameObject gameObject)
    {
        if (followingObjects.ContainsKey(key)) return;
        FollowingObject followingObject = new FollowingObject(name, gameObject);
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

    public void AddInactiveEnemy(GameObject gameObject)
    {
        disabledEnemies.Add(gameObject);
    }
    public void ReactivateEnemies()
    {

        foreach (GameObject enemy in disabledEnemies)
        {
            enemy.SetActive(true);
        }
        disabledEnemies.Clear();
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
        pointCounter.AddPointsToTotal((int)(pointsToAdd * sessionMultiplier), isCombo);
    }

    public void EndPointCombo()
    {
        pointCounter.EndCombo();
    }
}
