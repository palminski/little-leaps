using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private float fullHeight;
    [SerializeField] private Enemy enemy;

    private int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        fullHeight = transform.localScale.y;
        maxHealth = enemy.health;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy == null) Destroy(gameObject);
        transform.localScale = new Vector3(transform.localScale.x, fullHeight * (enemy.health / (float)maxHealth), transform.localScale.z);
    }
}
