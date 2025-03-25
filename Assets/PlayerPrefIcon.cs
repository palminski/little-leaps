using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefIcon : MonoBehaviour
{
    [SerializeField] private string prefString;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (image != null && PlayerPrefs.HasKey(prefString))
        {
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }
    }
}
