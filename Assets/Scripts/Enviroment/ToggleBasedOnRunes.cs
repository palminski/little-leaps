using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBasedOnRunes : MonoBehaviour
{
    public bool requiresPickupsToShow = true;
    [SerializeField] string[] otherPickupsInSet;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (string pickUp in otherPickupsInSet)
        {
            if (requiresPickupsToShow && !GameController.Instance.SessionCollectedObjects.Contains(pickUp)) {
                
                Destroy(gameObject);
                
            }

            if (!requiresPickupsToShow && GameController.Instance.SessionCollectedObjects.Contains(pickUp)) {
                
                Destroy(gameObject);
                
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
