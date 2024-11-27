using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public enum ActiveColor
    {
        Both,
        Purple,
        Green,
    }
    [SerializeField] private ActiveColor activeOn = ActiveColor.Both;

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

    private GameObject currentBullet;

    [SerializeField] bool canMultishoot = false;


    [SerializeField]
    private Vector2 direction = new(1f, 0f);

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        nextShotTime = shotInterval;
    }

    // Update is called once per frame
    void Update()
    {

        // if (!spriteRenderer.isVisible || BulletStillExists())
        // {
        //     // nextShotTime = Time.time + shotInterval;
        //     spriteRenderer.color = startingColor;
        // }

        if (nextShotTime >= (shotInterval - Mathf.Abs(blinkStart)) && CanShoot())
        {
            Blink();
        }
        else
        {
            spriteRenderer.color = startingColor;
        }

        if (nextShotTime >= shotInterval)
        {

            if (CanShoot())
            {
                currentBullet = GameController.Instance.PullFromPool(bullet, transform.position);

                currentBullet.GetComponent<bullet>().TargetPoint(transform.position + new Vector3(transform.lossyScale.x * direction.x, direction.y, 0));
                spriteRenderer.color = startingColor;

                Collider2D bulletCollider = currentBullet.GetComponent<Collider2D>();
                Collider2D enemyCollider = GetComponent<Collider2D>();
                if (enemyCollider)
                {
                    Physics2D.IgnoreCollision(bulletCollider, enemyCollider);
                }

                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent<Collider2D>(out var collider))
                    {
                        Physics2D.IgnoreCollision(bulletCollider, collider);
                    }
                }
            }


            nextShotTime = 0;

        }
        nextShotTime += Time.deltaTime;

    }

    private bool BulletStillExists()
    {
        return currentBullet != null && currentBullet.activeSelf;
    }

    private void Blink()
    {
        if (nextBlinkTime >= blinkInterval)
        {
            spriteRenderer.color = (spriteRenderer.color == startingColor) ? blinkColor : startingColor;
            nextBlinkTime = 0;
        }
        nextBlinkTime += Time.deltaTime;
    }

    private bool CanShoot()
    {
        if (!canMultishoot && BulletStillExists())
        {
            return false;
        }

        return (
                activeOn == ActiveColor.Both ||
                (activeOn == ActiveColor.Purple && GameController.Instance.RoomState == RoomColor.Purple) ||
                (activeOn == ActiveColor.Green && GameController.Instance.RoomState == RoomColor.Green)
            );
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, direction.normalized, Color.cyan);
    }
}
