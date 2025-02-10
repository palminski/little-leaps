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

[SerializeField] GameObject jumpBadge;
[SerializeField] GameObject dashBadge;
private float chargeMeterWidth;

private Player player;

private Canvas canvas;

private void OnEnable() {
        GameController.Instance.OnUpdateHUD += HandleUpdateHUD;
        GameController.Instance.OnUpdateDashIcon += HandleUpdateDashIcon;
        GameController.Instance.OnUpdateJumpIcon += HandleUpdateDoubleJumpIcon;
    }
    private void OnDisable() {
        GameController.Instance.OnUpdateHUD -= HandleUpdateHUD;
        GameController.Instance.OnUpdateDashIcon -= HandleUpdateDashIcon;
        GameController.Instance.OnUpdateJumpIcon -= HandleUpdateDoubleJumpIcon;
    }

    void Awake(){
        canvas = GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        canvas.worldCamera = Camera.main;
        if (chargeMeter) chargeMeterWidth = chargeMeter.rect.width;
        HandleUpdateHUD();
        if (dashBadge) {
            dashBadge.SetActive(false);
        }

        if (jumpBadge) {
            jumpBadge.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        canvas.enabled = false;
        canvas.enabled = true;
    }

    private void HandleUpdateHUD() {
    
        if (scoreText) scoreText.text = GameController.Instance.Score.ToString();
        if (healthText) {
            string healthString = "EGO: ";
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

    private void HandleUpdateDashIcon() {
        if (dashBadge) {
            dashBadge.SetActive(player.CanDash());
        }
    }

    private void HandleUpdateDoubleJumpIcon() {
        if (jumpBadge) {
            jumpBadge.SetActive(player.CanDoubleJump());
        }
    }
}
