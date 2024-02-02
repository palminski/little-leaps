using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    [SerializeField]
    private LevelConnection levelConnection;

    [SerializeField]
    private string targetSceneName;

    [SerializeField]
    private Transform spawnPoint;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (levelConnection == LevelConnection.ActiveConnection)
        {
            player.transform.position = spawnPoint.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // private void OnCollisionEnter2D(Collision2D collision) {
    //     SceneManager.LoadScene(targetSceneName);
    // }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {

        if (hitCollider.gameObject == player)
        {

            LevelConnection.ActiveConnection = levelConnection;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
