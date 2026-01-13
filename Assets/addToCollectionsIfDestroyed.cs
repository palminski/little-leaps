using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class addToCollectionsIfDestroyed : MonoBehaviour
{

    private string objectId;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectId = $"{SceneManager.GetActiveScene().buildIndex}{transform.position.x}{transform.position.y}";
        if (GameController.Instance.CollectedObjects.Contains(objectId)) Destroy(gameObject);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        if (GameController.Instance == null) return;
        if (!GameController.Instance.CollectedObjects.Contains(objectId)) GameController.Instance.TagObjectStringAsCollected(objectId);
    }
}
