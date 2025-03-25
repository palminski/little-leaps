using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class bullet : MonoBehaviour
{

    public float speed;
    private Vector3 direction;

    public ParticleSystem ps;
    [SerializeField] public GameObject shatterObject;

    void OnEnable()
    {
        ps.Clear();
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

            
            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            Player hitPlayer = hitCollider.GetComponent<Player>();
            hitPlayer.Damage(1);
            ps.transform.SetParent(null,worldPositionStays: true);
            gameObject.SetActive(false);
            if (shatterObject) GameController.Instance.PullFromPool(shatterObject, transform.position);
        }
        else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Solid"))
        {

            ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ps.transform.SetParent(null,worldPositionStays: true);
            gameObject.SetActive(false);
            if (shatterObject) GameController.Instance.PullFromPool(shatterObject, transform.position);
        }

        

    }

}
