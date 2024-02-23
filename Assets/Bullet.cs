using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{

    public float speed;
    private Vector3 direction;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    public void TargetPoint(Vector3 target) {
        direction = Vector3.Normalize(target - transform.position);
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        gameObject.SetActive(false);
    }
}
