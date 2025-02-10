using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TossBrokenObject : MonoBehaviour
{

    [SerializeField] private Vector2 spawnForce;
    [SerializeField] private float fadeDuration = 1f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(spawnForce.normalized * Random.Range(250f,350f));
        rb.AddTorque(Random.Range(8f,12f));
        StartCoroutine(FadeOutAndDestroy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FadeOutAndDestroy()
    {
        Color originalColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, spawnForce.normalized, Color.cyan);
    }
}
