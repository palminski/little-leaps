using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private Vignette vignette;

    [Header("Chromatic Abberation")]
    private ChromaticAberration chromaticAberration;
    [SerializeField] float pulseSpeed = 1f;
    [SerializeField] float maxAbberation = 1f;

    [Header("Film Grain")]
    private FilmGrain filmGrain;
    [SerializeField] float filmGrainReturnSpeed = 1f;

    private bool shouldPulse;
    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out filmGrain);
     
        shouldPulse = false;
        
     
        UpdateColor();
    }


    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
        GameController.Instance.OnPlayerDamaged += HandlePlayerDamaged;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
        GameController.Instance.OnPlayerDamaged -= HandlePlayerDamaged;
    }
    // Update is called once per frame
    void Update()
    {
        Pulse();

        if (filmGrain.response.value < 1)
        {
            filmGrain.response.value += filmGrainReturnSpeed * Time.deltaTime;
            if (filmGrain.response.value > 1) filmGrain.response.value = 1;
        }
    }
    private void HandleRoomStateChange()
    {
        shouldPulse = true;
        chromaticAberration.active = true;
        UpdateColor();
    }

    private void HandlePlayerDamaged()
    {
        filmGrain.response.value = 0;
        shouldPulse = true;
        chromaticAberration.active = true;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (GameController.Instance.RoomState == RoomColor.Purple)
        {
            vignette.color.value = GameController.ColorForPurple;
        }
        else
        {
            vignette.color.value = GameController.ColorForGreen;
        }
    }

    private void Pulse()
    {
        if (!chromaticAberration.active) return;
        if (shouldPulse && chromaticAberration.intensity.value < maxAbberation)
        {
            chromaticAberration.intensity.value += pulseSpeed * Time.deltaTime;
            return;
        }
        shouldPulse = false;
        chromaticAberration.intensity.value -= pulseSpeed * Time.deltaTime;
        if (chromaticAberration.intensity.value <= 0)
        {
            chromaticAberration.intensity.value = 0;
            chromaticAberration.active = false;
        }

    }
}
