using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bullet : MonoBehaviour
{

    public float speed;
    private Vector3 direction;

    public ParticleSystem ps;

    void OnEnable()
    {
        ps.transform.position = transform.position;
        ps.transform.SetParent(transform);
        ps.Play();
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
    }

    public void TargetPoint(Vector3 target)
    {
        direction = Vector3.Normalize(target - transform.position);
    }

    void OnTriggerEnter2D(Collider2D hitCollider)
    {
        if (hitCollider.gameObject.CompareTag("PlayerAttack"))
        {
            return;
        }
        if (hitCollider.gameObject.CompareTag("Player"))
        {
            Player player = hitCollider.gameObject.GetComponent<Player>();
            int directionToShove = (player.transform.position.x > transform.position.x) ? 1 : -1;
            player.Damage(1, directionToShove);
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ps.transform.SetParent(null);
            gameObject.SetActive(false);
        }
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ps.transform.SetParent(null);
            gameObject.SetActive(false);
        }

        

    }
}
