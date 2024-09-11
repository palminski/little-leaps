using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUp : MonoBehaviour
{
    private string id;
    [SerializeField] public string pickupName;
    private GameObject player;
    [SerializeField]private GameObject followItem;
    // Start is called before the first frame update
    void Start()
    {
        id = $"{pickupName}{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id)) Destroy(gameObject);
        if (GameController.Instance.FollowingObjects.ContainsKey(pickupName)) Destroy(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            GameController.Instance.TagObjectStringAsCollected(id);
            GameObject obj = Instantiate(followItem, transform.position, Quaternion.identity);
            obj.transform.SetParent(GameController.Instance.transform);
            GameController.Instance.AddFollowingObjects(pickupName, obj);
            Destroy(gameObject);
            
        }
    }
}
