using System.Collections;
using System.Collections.Generic;
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
            player.GetComponent<Player>().startPosition = spawnPoint.position;
            
            var camera = Camera.main.GetComponent<CameraControls>();
            if (camera && camera.canMove && !camera.onlyUp) camera.SnapToPosition(spawnPoint);
            player.transform.localScale = new(Mathf.Sign(spawnPoint.localPosition.x),1,1);
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
            
            GameController.Instance.ChangeScene(targetSceneName);
        }
    }
}
