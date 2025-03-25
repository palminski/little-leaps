using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLiftDoorOnTrigger : MonoBehaviour
{
    [SerializeField] private TriggerEvent CloseEvent;
    
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (CloseEvent)
        {
            CloseEvent.OnEventRaised.AddListener(OnEventRaised);
        }
    }
    private void OnDisable()
    {
        if (CloseEvent)
        {
            CloseEvent.OnEventRaised.RemoveListener(OnEventRaised);
        }
    }

    

    void OnEventRaised()
    {
        
    animator.SetTrigger("Close Lift");
        
    }

}
