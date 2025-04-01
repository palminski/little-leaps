using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnPermKey : MonoBehaviour
{
    [SerializeField] private string requiredKey;
    // Start is called before the first frame update
    void Start()
    {
         var permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;
        if(permCollected.Contains(requiredKey))
        {
            Destroy(gameObject);
        }
    }


}
