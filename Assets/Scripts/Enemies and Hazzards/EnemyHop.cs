using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHop : MonoBehaviour
{
    [SerializeField] private float jumpPower = 1f;

    [SerializeField] private float gravity = 0.08f;

    [SerializeField] private float JumpInterval = 1f;
    private float jumpTimer = 0f;

    private MovementCollisionHandler movementCollisionHandler;
    private Collider2D collider2d;
    private int direction;
    private Vector3 velocity;
    private Enemy enemy;
    private Animator animator;
    [SerializeField] private Animator spikesAnimator;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        enemy = GetComponent<Enemy>();
        collider2d = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) velocity.y = 0;

        velocity.y -= gravity;

        if (jumpTimer > JumpInterval && movementCollisionHandler.collisionInfo.below )
        {
            if (spikesAnimator) spikesAnimator.SetBool("Jumping", true);
            animator.SetBool("Jumping", true);
            jumpTimer = 0;
            movementCollisionHandler.Move(new(0,0.5f, 0));
            velocity.y = jumpPower;
        }
        else if(movementCollisionHandler.OnGround())
        {
            if (spikesAnimator) spikesAnimator.SetBool("Jumping", false);
            animator.SetBool("Jumping", false);
            jumpTimer += Time.deltaTime;
        }

        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
    }

    private IEnumerator WaitCheckAndDamage()
    {
        yield return new WaitForFixedUpdate();

        if (movementCollisionHandler.InGround()) 
        {
            enemy.KillEnemy();
            StopAllCoroutines();
        }
    }
}
