using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtPlayer : MonoBehaviour
{
    public enum ActiveColor
    {
        Both,
        Purple,
        Green,
    }
    // [SerializeField] private ActiveColor activeOn = ActiveColor.Both;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float blinkInterval = 0.0001f;
    [SerializeField] private float blinkStart = 0.5f;
    [SerializeField] private Color blinkColor;
    private float nextBlinkTime;
    private Color startingColor;
    [SerializeField] private float shotInterval = 1;
    private float nextShotTime;
    [SerializeField] private GameObject bullet;
    private GameObject player;
    private GameObject currentBullet;
    [SerializeField] bool canMultishoot = false;
    [SerializeField] Transform shotStartTransform;
    [SerializeField] private Vector2 direction = Vector2.right; 
    [SerializeField] private float fieldOfView = 120f;
    private Vector2 currentTarget = Vector2.zero;
    [SerializeField] private LayerMask opaqueLayers;

    public bool shouldShoot = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player");
        nextShotTime = 0;
    }

    void LateUpdate()
    {
print(CheckForPlayer());
        if (CheckForPlayer())
        {
            currentTarget = player.transform.position;
            nextShotTime += Time.deltaTime;
        }
        else if (nextShotTime >= (shotInterval - Mathf.Abs(blinkStart)) && CanShoot())
        {
            nextShotTime += Time.deltaTime;
        }

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
                ShootBulletAtPlayer();
            }
            nextShotTime = 0;
        }


    }
    private bool CheckForPlayer()
    {
        if (player == null) return false;
        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(direction.normalized * new Vector2(transform.localScale.x, 1), directionToPlayer);
        if (angleToPlayer > fieldOfView / 2f) return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, opaqueLayers);
        if (hit.collider != null) return false;

        return true;
    }

    public void ShootBulletAtPlayer()
    {
        if (AudioController.Instance != null) AudioController.Instance.PlayEnemyShot();
        currentBullet = GameController.Instance.PullFromPool(bullet, shotStartTransform ? shotStartTransform.position : transform.position);

        currentBullet.GetComponent<bullet>().TargetPoint(currentTarget);
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
        currentTarget = Vector2.zero;
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
        return shouldShoot;
        // return (
        //         activeOn == ActiveColor.Both ||
        //         (activeOn == ActiveColor.Purple && GameController.Instance.RoomState == RoomColor.Purple) ||
        //         (activeOn == ActiveColor.Green && GameController.Instance.RoomState == RoomColor.Green)
        //     );
    }

    private void OnDrawGizmos()
    {

        // Draw field of vision cone
        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fieldOfView / 2) * (direction.normalized * new Vector2(transform.localScale.x, 1)) * 2f;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fieldOfView / 2) * (direction.normalized * new Vector2(transform.localScale.x, 1)) * 2f;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
