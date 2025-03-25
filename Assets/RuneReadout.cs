using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneReadout : MonoBehaviour
{
    private List<LockRune> digits = new List<LockRune>();
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            LockRune lockrune = child.GetComponent<LockRune>();
            if (lockrune != null) digits.Add(lockrune);
        }
        print(digits.Count);
        for (int i = 0; i < digits.Count; i++)
        {
            print(digits[i]);
            if (GameController.Instance.SectionChips > i)
            {
                LockRune digit = digits[i];
                if (digit!= null)digit.LightUp();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
