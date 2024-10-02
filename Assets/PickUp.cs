using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickUp : MonoBehaviour
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
        if (GameController.Instance.FollowingObjects.ContainsKey(id)) DeActivate();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject == player)
        {
            GameObject obj = Instantiate(followItem, transform.position, Quaternion.identity);

            SpriteRenderer thisSR = GetComponent<SpriteRenderer>();
            SpriteRenderer newSR =obj.GetComponent<SpriteRenderer>();

            newSR.sprite = thisSR.sprite;
            newSR.color = thisSR.color;
            newSR.flipX = thisSR.flipX;
            newSR.flipY = thisSR.flipY;
            newSR.sortingLayerID = thisSR.sortingLayerID;
            newSR.sortingOrder = thisSR.sortingOrder;

            obj.transform.SetParent(GameController.Instance.transform);
            GameController.Instance.AddFollowingObjects(id, pickupName, obj);
            DeActivate();

        }
    }

    private void DeActivate()
    {
        Component[] components = GetComponents<Component>();
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
        Component[] components = GetComponents<Component>();
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
