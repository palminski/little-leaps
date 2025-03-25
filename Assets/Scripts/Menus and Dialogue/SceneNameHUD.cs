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
        if (debug) 
        {
            text.text = SceneManager.GetActiveScene().name;
        }
        else 
        {
            text.text = GlobalConstants.checkpointToSector.ContainsKey(GameController.Instance.Checkpoint) ? GlobalConstants.checkpointToSector[GameController.Instance.Checkpoint] : "???";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
