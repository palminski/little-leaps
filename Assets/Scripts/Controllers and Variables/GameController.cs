using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// using UnityEditor.Build.Content;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting;
using Steamworks;

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

    private Coroutine changingScenesCoroutine;
    private int score;
    public int Score
    {
        get { return score; }
    }

    //This is for level wipe timer
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

    //This is for speed running timer, naming is bad, but its already been coded like this
    private bool countUpTimerMoving;
    public bool CountUpTimerMoving
    {
        get { return countUpTimerMoving; }
    }
    private float timer;
    public float Timer
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

    [SerializeField] private string checkpointBackend;
    public string CheckpointBackend
    {
        get { return checkpointBackend; }
    }

    [SerializeField] private bool shouldSkipGameOver;
    public bool ShouldSkipGameOver
    {
        get { return shouldSkipGameOver; }
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

    private int sessionInstability = 0;
    public int SessionInstability
    {
        get { return sessionInstability; }
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

    private int sectionChips = 0;
    public int SectionChips
    {
        get { return sectionChips; }
    }

    private Dictionary<string, FollowingObject> followingObjects = new Dictionary<string, FollowingObject>();
    public Dictionary<string, FollowingObject> FollowingObjects
    {
        get { return followingObjects; }
    }

    private Dictionary<string, Sprite> savedSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> SavedSprites
    {
        get { return savedSprites; }
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
            ApplyGraphicsSettings();

            Instance = this;
            DontDestroyOnLoad(gameObject);
            collectedObjects = SaveDataManager.LoadGameData().collectedObjects ?? new HashSet<string>();
            SceneManager.sceneLoaded += OnSceneLoad;
            UpdateMultiplier(0, 8, 50);

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

        if (countUpTimerMoving && timer < 5940)
        {
            timer += Time.deltaTime;
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            OpenPauseMenu();
        }
    }

    public void ApplyGraphicsSettings()
    {
        Application.targetFrameRate = PlayerPrefs.HasKey("TargetFPS") ? PlayerPrefs.GetInt("TargetFPS") : 0;
        QualitySettings.vSyncCount = 0;
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
                OldFollowPlayer followPlayer = entry.Value.Object.GetComponent<OldFollowPlayer>();
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
        if (AudioController.Instance != null) AudioController.Instance.PlayShift();
        roomState = roomState == RoomColor.Purple ? RoomColor.Green : RoomColor.Purple;
        OnRoomStateChanged?.Invoke();
        return roomState;
    }

    public RoomColor SetRoomState(RoomColor roomColor)
    {
        roomState = roomColor;
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

        UpdateMultiplier(0, 8, 50);
        sectionChips = 0;
        charge = 0;
        bonusTimer = 0;
        timerMoving = false;
        sessionInstability = 0;
        savedSprites.Clear();

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

    public void UpdateMultiplier(int newPrestige, int newMaxHealth, float newHealing)
    {
        sessionPrestige = newPrestige;
        sessionHealing = newHealing;
        maxHealth = newMaxHealth;
        health = Mathf.Min(health, maxHealth);
        sessionMultiplier = GlobalConstants.getMultiplier(sessionPrestige, maxHealth, sessionHealing);
    }

    public void UpdateInstability(int instabilityToAdd)
    {
        sessionInstability += instabilityToAdd;
        sessionInstability = Mathf.Clamp(sessionInstability, 0, 4);
    }

    public void UpdateSectionChips(int chipsToAdd)
    {
        sectionChips += chipsToAdd;
        sectionChips = Mathf.Clamp(sectionChips, 0, 3);
    }

    public void SetCheckPoint(string scene)
    {
        checkpoint = scene;
    }

    public void SetCheckPointBackend(string scene)
    {
        checkpointBackend = scene;
    }

    public void SetShouldSkipGameover(bool shouldSkip)
    {
        shouldSkipGameOver = shouldSkip;
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
        if (SteamManager.Initialized)
        {
            if (score > 77600)
            {
                SteamUserStats.SetAchievement("ACH_HIGH_SCORE");
                SteamUserStats.StoreStats();
            }
            if (score >= 1000000)
            {
                SteamUserStats.SetAchievement("ACH_MILLION");
                SteamUserStats.StoreStats();
            }
            if (score >= 2000000)
            {
                SteamUserStats.SetAchievement("ACH_MULTI_MILLION");
                SteamUserStats.StoreStats();
            }

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
        // if (health <= 0) return 0;
        if (health + healthChange > maxHealth) return 0;
        if (healthChange < 0) OnPlayerDamaged?.Invoke();

        //edge case for only 1 max health
        bool willHealAtOneHealth = false;
        if (maxHealth == 1 && charge >= ChargeMax)
        {
            willHealAtOneHealth = true;
            charge = 0;
        }
        if (!willHealAtOneHealth) health += healthChange;

        if (healthChange <= 0) ChangeCharge(0);
        health = Mathf.Clamp(health, 0, maxHealth);
        OnUpdateHUD?.Invoke();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Player playerScript = player ? player.GetComponent<Player>() : null;

        if (health <= 0)
        {
            if (AudioController.Instance != null) AudioController.Instance.PlayPlayerKilled();
            ClearFollowingObjects();

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
            if (AudioController.Instance != null) AudioController.Instance.PlayPlayerKilled();
            ClearFollowingObjects();
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

    public void StartSpeedRunTimer()
    {
        if (!PlayerPrefs.HasKey("ShowSpeedRunTimer")) return;
        if (countUpTimerMoving) return;
        timer = 0;
        countUpTimerMoving = true;
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

    public void StopSpeedRunTimer()
    {
        countUpTimerMoving = false;
        timer = 0;
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
        StopTimer();
        StopSpeedRunTimer();
        player.SetActive(true);

        // GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject _deathObject = Instantiate(deathObject, player.transform.position, Quaternion.identity);
        if (AudioController.Instance != null) AudioController.Instance.PlayGameOver();
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

    public void RemoveCharge()
    {
        charge = 0;
    }

    public void OpenPauseMenu()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null) return;
        if (AudioController.Instance != null) AudioController.Instance.PlaySelect();

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
                OldFollowPlayer followPlayer = entry.Value.Object.GetComponent<OldFollowPlayer>();
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

    public void ClearFollowingObjects()
    {
        foreach (KeyValuePair<string, FollowingObject> entry in followingObjects)
        {
            if (entry.Value.Object)
            {
                Destroy(entry.Value.Object);
            }
        }
        followingObjects.Clear();
    }

    public void AddSavedSprite(string key, Sprite sprite)
    {
        if (savedSprites.ContainsKey(key)) return;
        savedSprites.Add(key, sprite);
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

    public void ChangeScene(string sceneName, bool playLIftNoise = false)
    {
        if (changingScenesCoroutine != null) return;
        levelChangerAnimator.SetTrigger("FadeIn");
        changingScenesCoroutine = StartCoroutine(ChangeSceneAfterDelay(sceneName, playLIftNoise));
    }
    public IEnumerator ChangeSceneAfterDelay(string sceneName, bool playLIftNoise = false)
    {
        yield return null;
        AnimatorStateInfo stateInfo = levelChangerAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        if (playLIftNoise && AudioController.Instance) AudioController.Instance.PlayLiftClose();
        SceneManager.LoadScene(sceneName);
        changingScenesCoroutine = null;
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
