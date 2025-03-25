using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWobbleFloat : MonoBehaviour
{
    [SerializeField] private float verticalAmplitude = 5f;
    [SerializeField] private float horizontalAmplitude = 5f;
    [SerializeField] private float verticalDuration = 5f;
    [SerializeField] private float horizontalDuration = 5f;
    [SerializeField][Range(0, 2)] private float verticalEase = 0.5f;
    [SerializeField][Range(0, 2)] private float horizontalEase = 0.5f;

    private Vector3 startPosition;
    private float vTime;
    private float hTime;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        hTime += Time.deltaTime / horizontalDuration;
        vTime += Time.deltaTime / verticalDuration;

        float horizontalPercent = Mathf.PingPong(hTime, 1f);
        float verticalPercent = Mathf.PingPong(vTime, 1f);

        float easedHorizontalPercent = Ease(horizontalPercent, horizontalEase);
        float easedVerticalPercent = Ease(verticalPercent, verticalEase);

        float newX = startPosition.x + Mathf.Lerp(-horizontalAmplitude, horizontalAmplitude, easedHorizontalPercent);
        float newY = startPosition.y + Mathf.Lerp(-verticalAmplitude, verticalAmplitude, easedVerticalPercent);

        transform.position = new Vector3(newX, newY, startPosition.z);
    }

    public float Ease(float x, float easeAmount)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.magenta;
        float size = 0.3f;
        Gizmos.DrawSphere(transform.position + new Vector3(-horizontalAmplitude, 0, 0), size);
        Gizmos.DrawSphere(transform.position + new Vector3(horizontalAmplitude, 0, 0), size);


        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + new Vector3(0, -verticalAmplitude, 0), size);
        Gizmos.DrawSphere(transform.position + new Vector3(0, verticalAmplitude, 0), size);
    }
}

