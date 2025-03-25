using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(MovementCollisionHandler))]
public class EnemyFloatAimless : MonoBehaviour
{
    [SerializeField]
    public float speed = 0.35f;

    [SerializeField]
    public Vector2 startDirection;
    
    private Vector2 direction = new(1,1);
    
    private Enemy enemy;

    private MovementCollisionHandler movementCollisionHandler;
    
    
    private Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        movementCollisionHandler = GetComponent<MovementCollisionHandler>();
        enemy = GetComponent<Enemy>();
    

        direction = startDirection.normalized;    
    }

    void FixedUpdate()
    {


        velocity = direction * speed;


        //Pass the velocity to the movement and collision handler
        //=========================================================
        movementCollisionHandler.Move(velocity);
        
        if (movementCollisionHandler.collisionInfo.right || movementCollisionHandler.collisionInfo.left) direction.x = -direction.x;
        if (movementCollisionHandler.collisionInfo.above || movementCollisionHandler.collisionInfo.below) direction.y = -direction.y;

    

    }

    // Update is called once per frame
    void Update()
    {

        if (direction.x != 0)
        {
            Vector3 newScale = new(Mathf.Sign(direction.x), 1, 1);
            transform.localScale = newScale;
        }
        if (movementCollisionHandler.InGround())
        {
            StartCoroutine(WaitCheckAndDamage());
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, startDirection.normalized, Color.cyan);
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
