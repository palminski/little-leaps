using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ToggleOnInterval : MonoBehaviour
{

    [SerializeField] private float interval = 1;
    [SerializeField] private bool startActive = false;
    private float swapCounter;
    private bool active = false;
    private Tilemap tilemap;
    private Color activeColor;
    private Color deactiveColor;

    private Collider2D tileCollider;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileCollider = GetComponent<Collider2D>();
        activeColor = tilemap.color;
        deactiveColor = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.2f);
        
        //This looks dumb, but Swap() will immediatly switch it to its opposite
        if (!startActive) active = true;
        Swap();
    }

    // Update is called once per frame
    void Update()
    {
        swapCounter += Time.deltaTime;
        if (swapCounter >= interval)
        {
            swapCounter = 0;
            Swap();
        }
    }

    private void Swap()
    {
        // UpdateColor();
        if (active)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    private void Activate()
    {
        if (AudioController.Instance) AudioController.Instance.PlayBlueBlokcs();
        active = true;
        if (tilemap) tilemap.color = activeColor;
        if (tileCollider) tileCollider.enabled = true;
    }

    private void Deactivate()
    {
        active = false;
        if (tilemap) tilemap.color = deactiveColor;
        StartCoroutine(WaitThenRemoveCollision());
        
    }

    private IEnumerator WaitThenRemoveCollision() {
        yield return new WaitForSeconds(0.05f);
        if (tileCollider) tileCollider.enabled = false;
    }

}
