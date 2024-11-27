using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBar : MonoBehaviour
{
    private float fullHeight;
    private float countDownMax;
    private float countDown;
    // Start is called before the first frame update
    void Start()
    {
        fullHeight = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (countDown > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, fullHeight * (countDown / countDownMax), transform.localScale.z);
            countDown -= Time.deltaTime;
        }
        if (transform.localScale.y < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        }
    }

    public void StartCountdown(float time)
    {
        countDownMax = time;
        countDown = time;
    }
}
