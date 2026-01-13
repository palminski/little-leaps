using UnityEngine;

public class HideIfSessionCollectablePresent : MonoBehaviour
{
    [SerializeField] private string stringToCheck;

    void Awake()
    {
        if (GameController.Instance.SessionCollectedObjects.Contains(stringToCheck))
        {
            Destroy(gameObject);
        }
    }
}
