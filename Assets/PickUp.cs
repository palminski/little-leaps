using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    
    [SerializeField] public string pickupName;
    private GameObject player;
    [SerializeField]private GameObject followItem;
    // Start is called before the first frame update
    void Start()
    {
        
        if (GameController.Instance.CollectedObjects.Contains(pickupName)) Destroy(gameObject);
        if (GameController.Instance.FollowingObjects.ContainsKey(pickupName)) Destroy(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            
            GameObject obj = Instantiate(followItem, transform.position, Quaternion.identity);
            obj.transform.SetParent(GameController.Instance.transform);
            GameController.Instance.AddFollowingObjects(pickupName, obj);
            Destroy(gameObject);
            
        }
    }
}
