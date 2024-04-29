using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class ColorSwapper : MonoBehaviour
{
    private Color colorForRS0;

    private Color colorForRS1;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    private Image image;

    private TMP_Text text;
    private Light2D light2D;

    private ParticleSystem partSys;

    [SerializeField][Range(0,1)]
    private float brightnessForRS0 = 1f;

    [SerializeField][Range(0,1)]
    private float brightnessForRS1 = 1f;

    [SerializeField]
    private float maxAlphaSubtractionPercentage = 0.5f;

    [SerializeField]
    private float pulseSpeed = 1;

    [SerializeField]
    private bool shouldPulse;

    private float timeSinceLastBeat;

    public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        image = GetComponent<Image>();
        text = GetComponent<TMP_Text>();
        light2D = GetComponent<Light2D>();
        partSys = GetComponent<ParticleSystem>();
        
        colorForRS0 = GameController.ColorForPurple;
        colorForRS1 = GameController.ColorForGreen;

        colorForRS0 = Color.Lerp(colorForRS0, Color.black, (1-brightnessForRS0));
        colorForRS1 = Color.Lerp(colorForRS1, Color.black, (1-brightnessForRS1));

        UpdateColor();
        timeSinceLastBeat = Time.time;
    }

    void Update()
    {
        if (shouldPulse) {
            Pulse();
        }
    }

    private void OnEnable() {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        UpdateColor();
    }
    private void OnDisable() {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }

    private void UpdateColor() {
        if (GameController.Instance.RoomState == RoomColor.Purple) {
            if (spriteRenderer) spriteRenderer.color = colorForRS0;
            if (tilemap) tilemap.color = colorForRS0;
            if (image) image.color = colorForRS0;
            if (text) text.color = colorForRS0;
            if (light2D) light2D.color = colorForRS0;
            if (partSys) {
                UpdateParticles(colorForRS0);
                ParticleSystem.MainModule main = partSys.main;
                main.startColor = colorForRS0;
            } 
            
        }
        else {
            if (spriteRenderer) spriteRenderer.color = colorForRS1;
            if (tilemap) tilemap.color = colorForRS1;
            if (image) image.color = colorForRS1;
            if (text) text.color = colorForRS1;
            if (light2D) light2D.color = colorForRS1;
            if (partSys) {
                UpdateParticles(colorForRS1);
                ParticleSystem.MainModule main = partSys.main;
                main.startColor = colorForRS1;
            } 
        }
    }

    private void HandleRoomStateChange() {
        UpdateColor();
    }

    private void Pulse() {

        Color baseColor = GameController.Instance.RoomState == RoomColor.Purple ? colorForRS0 : colorForRS1;


        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);

        // Interpolate alpha between minAlpha and maxAlpha
        float alphaToSubtract = Mathf.Lerp(baseColor.a * maxAlphaSubtractionPercentage, 0, pulse);

        // Apply the modified alpha value to the tilemap color
        
        // baseColor.a -= alphaToSubtract;
        Color darkColor = Color.Lerp(baseColor, Color.black, maxAlphaSubtractionPercentage);
        Color lerpedColor = Color.Lerp(baseColor, darkColor, pulse);

        if (spriteRenderer) spriteRenderer.color = lerpedColor;
        if (tilemap) tilemap.color = lerpedColor;
        if (image) image.color = lerpedColor;
        if (text) text.color = lerpedColor;
    }

    private void UpdateParticles(Color color) {
        ParticleSystem.Particle[] currentParticles = new ParticleSystem.Particle[partSys.particleCount];
        int numParticlesAlive = partSys.GetParticles(currentParticles);

        for (int i = 0 ; i < numParticlesAlive; i++)
        {
            currentParticles[i].startColor = color;
        }

        partSys.SetParticles(currentParticles, numParticlesAlive);
    }
}
