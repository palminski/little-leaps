using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{

[SerializeField]
private TMP_Text scoreText;

[SerializeField]
private TMP_Text healthText;

private void OnEnable() {
        GameController.Instance.OnUpdateHUD += HandleUpdateHUD;
    }
    private void OnDisable() {
        GameController.Instance.OnUpdateHUD -= HandleUpdateHUD;
    }

    // Start is called before the first frame update
    void Start()
    {
        HandleUpdateHUD();
    }

    private void HandleUpdateHUD() {
    
        if (scoreText) scoreText.text = "Score: " + GameController.Instance.Score.ToString();
        if (healthText) healthText.text = "Health: " + GameController.Instance.Health.ToString();

    }
}
