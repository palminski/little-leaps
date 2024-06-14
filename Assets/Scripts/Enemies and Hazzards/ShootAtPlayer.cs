using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtPlayer : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float blinkInterval = 0.0001f;
    [SerializeField]
    private float blinkStart = 0.5f;
    [SerializeField]
    private Color blinkColor;
    private float nextBlinkTime;
    private Color startingColor;

    [SerializeField]
    private float shotInterval = 1;

    private float nextShotTime;

    [SerializeField]
    private GameObject bullet;

    private GameObject player;

    private GameObject currentBullet;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player");
        nextShotTime = shotInterval;
    }

    // Update is called once per frame
    void Update()
    {

        if (!spriteRenderer.isVisible || BulletStillExists()) {
            nextShotTime = Time.time + shotInterval;
            spriteRenderer.color = startingColor;
        }

        if (Time.time > (nextShotTime - Mathf.Abs(blinkStart)))
        {
            Blink();
        }

        if (Time.time > nextShotTime)
        {

            currentBullet = GameController.Instance.PullFromPool(bullet, transform.position);
            if (player) currentBullet.GetComponent<bullet>().TargetPoint(player.transform.position);
            spriteRenderer.color = startingColor;

            Collider2D bulletCollider = currentBullet.GetComponent<Collider2D>();
            Collider2D enemyCollider = GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(bulletCollider, enemyCollider);

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Collider2D>(out var collider))
                {
                    Physics2D.IgnoreCollision(bulletCollider, collider);
                }
            }

            nextShotTime = Time.time + shotInterval;
        }

    }

    private bool BulletStillExists() {
        return currentBullet != null && currentBullet.activeSelf;
    }

    private void Blink()
    {
        if (Time.time > nextBlinkTime)
        {
            spriteRenderer.color = (spriteRenderer.color == startingColor) ? blinkColor : startingColor;
            nextBlinkTime = Time.time + blinkInterval;
        }
    }
}
