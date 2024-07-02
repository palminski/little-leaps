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
        solidIndex = LayerMask.NameToLayer("SolidPlatform");
        gameObject.layer = solidIndex;
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
            StartCoroutine(MakeSolid());
        }
        else {
            gameObject.layer = passableIndex;
        }
    }

    private bool IsPlayerAbove(){
        if (!playerCollider) return false;
        float playerBottom = playerCollider.bounds.min.y;
        float platformTop = platformCollider.bounds.max.y;

        return playerBottom >= platformTop - GameController.GlobalSkinWidth;
    }

    public bool IsPassable() {
        if (!IsPlayerAbove()) {
            return true;
        }
        else {
            return false;
        }
    }

    //Player needs to finish calculating movement before we make it solid again, so we wait for the fixed update
    IEnumerator MakeSolid() {
        yield return new WaitForFixedUpdate();
        gameObject.layer = solidIndex;
    }

    
}
