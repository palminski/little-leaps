using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPermenantCollectible : MonoBehaviour
{
    [SerializeField] private string requiredKey;
    // Start is called before the first frame update
    void Start()
    {
         var permCollected = SaveDataManager.LoadGameData().permanentCollectedObjects;
        if(permCollected.Contains(requiredKey))
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0,0,0,0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
