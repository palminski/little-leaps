using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PointCounter : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private TMP_Text pointText;
    [SerializeField] private TMP_Text backgroundText;
    [SerializeField] private Vector3 offsetFromPlayer;
    private int currentPoints = 0;
    private int combo = 0;
    private bool canCombo = false;

    private bool stickToPlayer = false;
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (stickToPlayer) JumpToPlayer();
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        StopAllCoroutines();
        gameObject.SetActive(false);
        stickToPlayer = false;
        currentPoints = 0;
    }

    public void JumpToPlayer()
    {
        if (player) transform.position = player.transform.position + offsetFromPlayer;
    }

    public void AddPointsToTotal(int pointsToAdd, bool isCombo = true)
    {
        gameObject.SetActive(true);
        if (!canCombo)
        {
            StopAllCoroutines();
            if (isCombo){
                currentPoints = 0;
                stickToPlayer = false;
            } 
            if (!isCombo) stickToPlayer = true;
        }
        currentPoints += pointsToAdd + (isCombo ? pointsToAdd*combo : 0);
        if (isCombo)
        {
            GameController.Instance.AddToScore(pointsToAdd*combo);
            combo++;
        } 
        pointText.text = currentPoints.ToString();
        if (backgroundText) backgroundText.text = currentPoints.ToString();
        StartCombo();
        
    }

    public void StartCombo()
    {
        canCombo = true;
    }

    public void EndCombo()
    {
        if (canCombo == false) return;
        canCombo = false;
        combo = 0;
        if (gameObject.activeSelf) StartCoroutine(WaitAndHide());
    }

    private IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(0.7f);
        currentPoints = 0;
        gameObject.SetActive(false);
        stickToPlayer = false;
    }
}
