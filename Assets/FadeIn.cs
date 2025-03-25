using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FadeIn : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private TriggerEvent triggerEvent;
    [SerializeField] private WorldDialogue worldDialogue;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private ParticleSystem ps2;
    [SerializeField] private Rigidbody2D rabitRB;
    private bool isFInished = false;
    [SerializeField] float fadeSpeed = 0.01f;

    void Awake()
    {
        if (worldDialogue != null)
        {
            // worldDialogue.startingText = "FOOO";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.AddToScore(GameController.Instance.Score / 2);
        Camera.main.GetComponent<CameraControls>().canMove = false;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {

            player.SetActive(false);
        }

        sr = GetComponent<SpriteRenderer>();

        Color newColor = sr.color;


        newColor.a = 0;






        sr.color = newColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.color.a < 1)
        {
            Color newColor = sr.color;
            newColor.a += fadeSpeed * Time.deltaTime;
            sr.color = newColor;
        }
        else if (!isFInished)
        {
            Color newColor = sr.color;
            newColor.a = 1;
            sr.color = newColor;
            ps.Stop();
            isFInished = true;
            StartCoroutine(TypeAndEndGame());
        }

    }

    private IEnumerator TypeAndEndGame()
    {
        yield return new WaitForSeconds(3f);
        // rabitRB.bodyType = RigidbodyType2D.Dynamic;
        // rabitRB.AddForce(new(Random.Range(-100,100), -5));
        Destroy(ps.gameObject);
        ParticleSystem[] allPs = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem ps_ in allPs)
        {
            ps_.Stop();
        }
        triggerEvent.Raise();
        yield return new WaitForSeconds(5f);


        if (GameController.Instance.SessionMultiplier > 10)
        {
            worldDialogue.textElement.text = $@"
ERROR!!! - [ Synapse Instability ] - ERROR!!!
--------------------------------------------------------

ERROR_ !!!
VNTs Exceed Safe Concentration For Absorption,,,
Attempting RE--.[;]'?:---[CRITICAL ERROR]

--------------------------------------------------------
";
            if (rabitRB) {
                Destroy(rabitRB.gameObject);
            }
            yield return new WaitForSeconds(5f);
            GameController.Instance.UpdateHighScores("RABIT");

            SaveDataManager.AddPermanentCollectedString("RABIT escaped");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        yield return new WaitForSeconds(5f);

        GameController.Instance.ChangeScene("Game Complete");


    }
}
