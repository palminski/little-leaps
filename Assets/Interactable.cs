using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField]
    private float interactionDistance = 1f;

    [SerializeField]
    private Material outlineMaterial;

    [SerializeField]
    private Dialogue dialogueToStart;


    [SerializeField]
    private GameObject dialogueSystem;

    private SpriteRenderer spriteRenderer;
    private Material startingMaterial;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingMaterial = spriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanInteractWith()) {
            spriteRenderer.material = outlineMaterial;
        }
        else {
            spriteRenderer.material = startingMaterial;
        }
    }
    
    void OnSelect() {
        if (CanInteractWith() && (FindObjectOfType<DialogueManager>() == null)) {
            print("test");
            dialogueSystem.GetComponent<DialogueManager>().currentDialogue = dialogueToStart;
            Instantiate(dialogueSystem);
        }
    }

    private bool CanInteractWith() {
        if (Vector2.Distance(transform.position, playerTransform.position) <= interactionDistance) return true;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
