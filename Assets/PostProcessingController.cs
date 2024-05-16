using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    [SerializeField] float pulseSpeed = 1f;
    [SerializeField] float maxAbberation = 1f;

    private bool shouldPulse;
    // Start is called before the first frame update
    void Start()
    {
        shouldPulse = false;
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);
        UpdateColor();
    }


    private void OnEnable()
    {
        GameController.Instance.OnRoomStateChanged += HandleRoomStateChange;
    }
    private void OnDisable()
    {
        GameController.Instance.OnRoomStateChanged -= HandleRoomStateChange;
    }
    // Update is called once per frame
    void Update()
    {
        Pulse();
    }
    private void HandleRoomStateChange()
    {
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
