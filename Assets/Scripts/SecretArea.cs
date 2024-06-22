using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretArea : MonoBehaviour
{
    private Transform playerTransform;
    private Tilemap tilemap;
    [SerializeField] float detectionDistance = 1f;
    [SerializeField] float fadeSpeed = 3f;
    [SerializeField] float returnSpeed = 1.5f;
    [SerializeField] private LayerMask layerMask;
    private Color startingColor;
    [SerializeField] Color noticedOpacity;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        startingColor = tilemap.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerClose())
        {
            tilemap.color = Color.Lerp(tilemap.color, noticedOpacity, Time.deltaTime * fadeSpeed);
        }
        else
        {
            tilemap.color = Color.Lerp(tilemap.color, startingColor, Time.deltaTime * returnSpeed);
        }
    }

    private bool IsPlayerClose()
    {
        if (!playerTransform) return false;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerTransform.position, detectionDistance, layerMask);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }
}
