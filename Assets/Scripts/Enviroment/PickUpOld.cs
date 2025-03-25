using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUpOld : MonoBehaviour
{
    private string id;
    [SerializeField] public string pickupName;
    private GameObject player;
    [SerializeField] private GameObject followItem;
    // Start is called before the first frame update

    private void OnEnable()
    {
        GameController.Instance.OnPlayerDamaged += HandlePlayerDamaged;
    }
    private void OnDisable()
    {
        GameController.Instance.OnPlayerDamaged -= HandlePlayerDamaged;
    }

    void Start()
    {
        id = $"{pickupName}{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(id)) Destroy(gameObject);
        if (GameController.Instance.CollectedObjects.Contains(pickupName)) Destroy(gameObject);
        if (GameController.Instance.FollowingObjects.ContainsKey(id)) DeActivate();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            if(followItem) {
                GameObject followObject = Instantiate(followItem, transform.position, Quaternion.identity);

                // SpriteRenderer thisSR = GetComponent<SpriteRenderer>();
                // SpriteRenderer newSR =followObject.GetComponent<SpriteRenderer>();

                GameObject newObj = Instantiate(gameObject);

                PickUpOld newPickup = newObj.GetComponent<PickUpOld>();
                Rigidbody2D newRigidBody = newObj.GetComponent<Rigidbody2D>();
                Collider2D newCollider = newObj.GetComponent<Collider2D>();

                if(newPickup) newPickup.enabled = false;
                if(newRigidBody) newRigidBody.bodyType = RigidbodyType2D.Static;
                if(newCollider) newCollider.enabled = false;

                newObj.transform.SetParent(followObject.transform);
                // newSR.sprite = thisSR.sprite;
                // newSR.color = thisSR.color;
                // newSR.flipX = thisSR.flipX;
                // newSR.flipY = thisSR.flipY;
                // newSR.sortingLayerID = thisSR.sortingLayerID;
                // newSR.sortingOrder = thisSR.sortingOrder;

                followObject.transform.SetParent(GameController.Instance.transform);
                GameController.Instance.AddFollowingObjects(id, pickupName, followObject);
                DeActivate();
            }
            else
            {
                GameController.Instance.TagObjectStringAsCollected(pickupName);
            }
            

        }
    }

    private void DeActivate()
    {
        Component[] components = GetComponentsInChildren<Component>();
        foreach (Component component in components)
        {
            if (component != this)
            {
                if (component is Behaviour behaviour)
                {
                    behaviour.enabled = false;
                }
                if (component is SpriteRenderer renderer1)
                {
                    renderer1.enabled = false;
                }
            }
        }
    }

    private void ReActivate()
    {
        Component[] components = GetComponentsInChildren<Component>();
        foreach (Component component in components)
        {
            if (component != this)
            {
                if (component is Behaviour behaviour)
                {
                    behaviour.enabled = true;
                }
                if (component is SpriteRenderer renderer1)
                {
                    renderer1.enabled = true;
                }
            }
        }
    }

    void HandlePlayerDamaged()
    {
        if (!GameController.Instance.CollectedObjects.Contains(id)) ReActivate();
    }
}
