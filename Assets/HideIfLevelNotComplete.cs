using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfLevelNotComplete : MonoBehaviour
{
    [SerializeField] private string[] stringsToCheck;

    // Start is called before the first frame update
    void Awake()
    {

        CheckLevels();
    }

    void CheckLevels()
    {
        foreach (string stringToCheck in stringsToCheck)
        {
            if (GameController.Instance.SessionCollectedObjects.Contains(stringToCheck))
            {
                return;
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
