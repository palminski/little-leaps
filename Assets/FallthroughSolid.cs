using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FallthroughSolid : MonoBehaviour
{

    private PlayerControls playerControls;

    private  Collider2D playerCollider;
    private Collider2D platformCollider;

    private int passableIndex;
    private int solidIndex;



    // Start is called before the first frame update
    void Start() {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        platformCollider = GetComponent<Collider2D>();
    }
    void Awake()
    {
        playerControls = new PlayerControls();
        passableIndex = LayerMask.NameToLayer("PlayerCanPass");
        solidIndex = LayerMask.NameToLayer("Solid");
    }

    void OnEnable() {
        playerControls.Enable();
    }

    void OnDisable() {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControls.Movement.FastFall.IsPressed()) {
            gameObject.layer = passableIndex;
        }
        else if (IsPlayerAbove()) {
            gameObject.layer = solidIndex;
        }
        else {
            gameObject.layer = passableIndex;
        }
    }

    private bool IsPlayerAbove(){
        
        float playerBottom = playerCollider.bounds.min.y;
        float platformTop = platformCollider.bounds.max.y;

        return playerBottom >= platformTop - GlobalVars.globalSkinWidth;
    }

    public bool IsPassable() {
        if (!IsPlayerAbove()) {
            return true;
        }
        else {
            return false;
        }
    }

    
}
