using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GameLight : MonoBehaviour
{
    private Light2D light2D;
    private bool shouldFade = false;

    [SerializeField] private float fadeSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFade)
        {
            light2D.intensity -= fadeSpeed * Time.deltaTime;
            if (light2D.intensity <= 0) Destroy(gameObject);
        }
    }

    public void Fade()
    {
        shouldFade = true;
    }
}
