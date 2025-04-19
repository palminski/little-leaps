using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneNameHUD : MonoBehaviour
{
    [SerializeField] bool debug = false;
    private TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        string scene = SceneManager.GetActiveScene().name;
        if (debug) 
        {
            text.text = scene;
        }
        
        //     {"lv_6_1", "Sector 2-1"},
        //     {"lv_7_1", "Sector 2-3"},
        //     {"lv_8_1", "Sector 3-0"},
        //     {"lv_9_1", "Sector 3-3"},
        //     {"lv_10_1", "Sector 4-0"},
        //     {"lv_11_1", "Sector 0-2"},
        //     {"lv_12_1", "Sector 1-3"},
        //     {"lv_13_1", "Sector 1-1"},
        //     {"lv_14_1", "Sector 3-1"},
        //     {"lv_15_1", "Sector 4-2"},
        //     {"lv_16_1", "Sector 3-2"},
        //     {"lv_17_1", "Sector 4-1"},
        //     {"lv_18_1", "Sector 4-3"},
        //     {"lv_19_1", "Sector 2-0"},
        //     {"lv_20_1", "Sector 2-2"},
        else if(scene.Contains("lv_1_"))
        {
            text.text = "Sector 0-0";
        }
        else if(scene.Contains("lv_2_"))
        {
            text.text = "Sector 0-1";
        }
        else if(scene.Contains("lv_3_"))
        {
            text.text = "Sector 0-3";
        }
        else if(scene.Contains("lv_4_"))
        {
            text.text = "Sector 1-0";
        }
        else if(scene.Contains("lv_5_"))
        {
            text.text = "Sector 1-2";
        }
        else if(scene.Contains("lv_6_"))
        {
            text.text = "Sector 2-1";
        }
        else if(scene.Contains("lv_7_"))
        {
            text.text = "Sector 2-3";
        }
        else if(scene.Contains("lv_8_"))
        {
            text.text = "Sector 3-0";
        }
        else if(scene.Contains("lv_9_"))
        {
            text.text = "Sector 3-3";
        }
        else if(scene.Contains("lv_10_"))
        {
            text.text = "Sector 4-0";
        }
        else if(scene.Contains("lv_11_"))
        {
            text.text = "Sector 0-2";
        }
        else if(scene.Contains("lv_12_"))
        {
            text.text = "Sector 1-3";
        }
        else if(scene.Contains("lv_13_"))
        {
            text.text = "Sector 1-1";
        }
        else if(scene.Contains("lv_14_"))
        {
            text.text = "Sector 3-1";
        }
        else if(scene.Contains("lv_15_"))
        {
            text.text = "Sector 4-2";
        }
        else if(scene.Contains("lv_16_"))
        {
            text.text = "Sector 3-2";
        }
        else if(scene.Contains("lv_17_"))
        {
            text.text = "Sector 4-1";
        }
        else if(scene.Contains("lv_18_"))
        {
            text.text = "Sector 4-3";
        }
        else if(scene.Contains("lv_19_"))
        {
            text.text = "Sector 2-0";
        }
        else if(scene.Contains("lv_20_"))
        {
            text.text = "Sector 2-2";
        }
        else if(scene.Contains("lv_21_"))
        {
            text.text = "?????????";
        }
        else
        {
            text.text = "";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
