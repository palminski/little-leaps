using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PointCounter : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private TMP_Text pointText;
    private int currentPoints = 0;

    private bool canCombo = false;
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void JumpToPlayer()
    {
        if (player) transform.position = player.transform.position;
    }

    public void AddPointsToTotal(int pointsToAdd)
    {
        gameObject.SetActive(true);
        if (!canCombo)
        {
            StopAllCoroutines();
            currentPoints = 0;
        }
        currentPoints += pointsToAdd;
        pointText.text = currentPoints.ToString();
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
        if (gameObject.activeSelf) StartCoroutine(WaitAndHide());
    }

    private IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(0.7f);
        currentPoints = 0;
        gameObject.SetActive(false);
    }
}
