using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

[SerializeField]
private TMP_Text scoreText;

[SerializeField]
private TMP_Text healthText;

[SerializeField]
private RectTransform chargeMeter;
private float chargeMeterWidth;

private void OnEnable() {
        GameController.Instance.OnUpdateHUD += HandleUpdateHUD;
    }
    private void OnDisable() {
        GameController.Instance.OnUpdateHUD -= HandleUpdateHUD;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (chargeMeter) chargeMeterWidth = chargeMeter.rect.width;
        HandleUpdateHUD();
    }

    private void HandleUpdateHUD() {
    
        if (scoreText) scoreText.text = GameController.Instance.Score.ToString();
        if (healthText) {
            string healthString = "";
            for (int i = 0; i < GameController.Instance.MaxHealth; i++)
            {
                if(i < GameController.Instance.Health)
                {
                    healthString += "|";
                    continue;
                }
                healthString += "-";
            }
            healthText.text = healthString;
        } 
        if (chargeMeter)
        {
            
            float chargeMeterPercentage = (float)GameController.Instance.Charge / GameController.Instance.ChargeMax;
            chargeMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, chargeMeterWidth * chargeMeterPercentage);
        }

    }
}
