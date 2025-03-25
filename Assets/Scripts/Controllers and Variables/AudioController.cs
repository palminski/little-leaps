using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public AudioClip bgMusic;
    public bool shouldStopMusic = false;

    public float bgVolume = 1;

    [SerializeField] private AudioClip[] jumps;
    [SerializeField] private AudioClip[] dashes;
    [SerializeField] private AudioClip[] landings;
    [SerializeField] private AudioClip[] airJumps;
    [SerializeField] private AudioClip[] enemyDefeats;
    [SerializeField] private AudioClip[] enemyShots;
    [SerializeField] private AudioClip[] enemySpikeStart;
    [SerializeField] private AudioClip[] enemySpikeDeploy;
    [SerializeField] private AudioClip[] playerDeaths;
    [SerializeField] private AudioClip[] shifts;
    [SerializeField] private AudioClip[] coinNoises;
    [SerializeField] private AudioClip[] pickupNoises;
    [SerializeField] private AudioClip[] buttons;
    [SerializeField] private AudioClip[] pistons;
    [SerializeField] private AudioClip[] springs;
    [SerializeField] private AudioClip[] select;
    [SerializeField] private AudioClip[] moveCursor;
    [SerializeField] private AudioClip[] back;
    [SerializeField] private AudioClip[] typingBlips;
    [SerializeField] private AudioClip[] liftShuts;
    [SerializeField] private AudioClip[] liftMoves;
    [SerializeField] private AudioClip[] blueBlocks;
    [SerializeField] private AudioClip[] gameOvers;

    private AudioSource audioSource;
    private AudioSource typingAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance.bgMusic != bgMusic) 
            {
                Instance.bgMusic = bgMusic;
                Instance.UpdateAndPlayBGMusic();    
            }
            if (shouldStopMusic)
            {
                Instance.audioSource.Stop();
            }
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        typingAudioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }
    void Start()
    {
        UpdateAndPlayBGMusic();
    }

    //Player=========================================================================
    public void PlayJump()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(jumps.Length == 0) return;
        AudioClip soundEffect = jumps[Random.Range(0,jumps.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        // tempAudioSource.volume = 0.7f;

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayLandings()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(landings.Length == 0) return;
        AudioClip soundEffect = landings[Random.Range(0,landings.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        tempAudioSource.volume = 0.5f;

        tempAudioSource.pitch = Random.Range(0.6f, 0.8f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayAirJump()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(jumps.Length == 0) return;
        AudioClip soundEffect = jumps[Random.Range(0,jumps.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(1.9f, 2.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayDash()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(dashes.Length == 0) return;
        AudioClip soundEffect = dashes[Random.Range(0,dashes.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        tempAudioSource.volume = 0.7f;

        tempAudioSource.pitch = Random.Range(1.9f, 2.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayPlayerKilled()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(playerDeaths.Length == 0) return;
        AudioClip soundEffect = playerDeaths[Random.Range(0,playerDeaths.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayShift()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(shifts.Length == 0) return;
        AudioClip soundEffect = shifts[Random.Range(0,shifts.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        // tempAudioSource.volume = 0.9f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    //Enemy=========================================================================
    public void PlayEnemyKilled()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(enemyDefeats.Length == 0) return;
        AudioClip soundEffect = enemyDefeats[Random.Range(0,enemyDefeats.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayEnemyShot()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(enemyShots.Length == 0) return;
        AudioClip soundEffect = enemyShots[Random.Range(0,enemyShots.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayEnemySpikeStart()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(enemySpikeStart.Length == 0) return;
        AudioClip soundEffect = enemySpikeStart[Random.Range(0,enemySpikeStart.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayEnemySpikeDeploy()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(enemySpikeDeploy.Length == 0) return;
        AudioClip soundEffect = enemySpikeDeploy[Random.Range(0,enemySpikeDeploy.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    //Env============================================================================
    public void PlayButton()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(buttons.Length == 0) return;
        AudioClip soundEffect = buttons[Random.Range(0,buttons.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        // tempAudioSource.volume = 0.9f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayCoinNoise()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(coinNoises.Length == 0) return;
        AudioClip soundEffect = coinNoises[Random.Range(0,coinNoises.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(1.2f, 1.5f);
        tempAudioSource.volume = 0.6f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayPickupNoise()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(pickupNoises.Length == 0) return;
        AudioClip soundEffect = pickupNoises[Random.Range(0,pickupNoises.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(1.2f, 1.5f);
        tempAudioSource.volume = 0.6f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }
    public void PlayPistonNoise(float scale)
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(pistons.Length == 0) return;
        AudioClip soundEffect = pistons[Random.Range(0,pistons.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(1.2f, 1.5f);
        tempAudioSource.volume = scale * 0.6f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlaySpringNoise()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(springs.Length == 0) return;
        AudioClip soundEffect = springs[Random.Range(0,springs.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.volume = 0.6f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayBlueBlokcs()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(blueBlocks.Length == 0) return;
        AudioClip soundEffect = blueBlocks[Random.Range(0,blueBlocks.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }
    public void PlayLiftClose()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(liftShuts.Length == 0) return;
        AudioClip soundEffect = liftShuts[Random.Range(0,liftShuts.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }
    public void PlayLiftMove()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(liftMoves.Length == 0) return;
        AudioClip soundEffect = liftMoves[Random.Range(0,liftMoves.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    // UI======================================================================================
    public void PlayMoveCursor()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(moveCursor.Length == 0) return;
        AudioClip soundEffect = moveCursor[Random.Range(0,moveCursor.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        tempAudioSource.volume = 0.6f;

        // tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }
    public void PlaySelect()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(select.Length == 0) return;
        AudioClip soundEffect = select[Random.Range(0,select.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        tempAudioSource.pitch = 1.5f;
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }
    public void PlayBack()
    {
        if (PlayerPrefs.HasKey("SFXOff")) return;
        if(back.Length == 0) return;
        AudioClip soundEffect = back[Random.Range(0,back.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();

        // tempAudioSource.pitch = Random.Range(0.9f, 1.1f);
        tempAudioSource.PlayOneShot(soundEffect);
        Destroy(tempAudioSource,soundEffect.length);
    }

    public void PlayTypingBeep()
    {
        if(typingBlips.Length == 0 || PlayerPrefs.HasKey("SFXOff")) return;
        AudioClip soundEffect = typingBlips[Random.Range(0,typingBlips.Length)];
        typingAudioSource.volume = 0.1f;
        typingAudioSource.pitch = Random.Range(0.87f, 0.93f);
        typingAudioSource.PlayOneShot(soundEffect);
    }

    public void PlayGameOver()
    {
        if(gameOvers.Length == 0 || PlayerPrefs.HasKey("MusicOff")) return;
        AudioClip soundEffect = gameOvers[Random.Range(0,gameOvers.Length)];
        AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
        tempAudioSource.PlayOneShot(soundEffect);
    }

    // public void StartTyping()
    // {
    //     if(back.Length == 0) return;
    //     AudioClip soundEffect = back[Random.Range(0,back.Length)];

    //     typingAudioSource = gameObject.AddComponent<AudioSource>();
    //     typingAudioSource.loop = true;
    //     typingAudioSource.clip = soundEffect;
    //     typingAudioSource.Play();
        
    // }

    // public void StopStyping()
    // {
    //     if (typingAudioSource == null) return;
    //     typingAudioSource.Stop();
    //     Destroy(typingAudioSource);
    // }

    //MUSIC===========================================================================
    public void UpdateAndPlayBGMusic() {
        audioSource.Stop();
        audioSource.volume = bgVolume;
        audioSource.clip = bgMusic;
        if (audioSource.clip != null && !PlayerPrefs.HasKey("MusicOff"))
        {
            audioSource.Play();
        }
    }

    //Settings============================================================================
    public void ToggleMusic()
    {
        if (PlayerPrefs.HasKey("MusicOff"))
        {
            PlayerPrefs.DeleteKey("MusicOff");
            PlayerPrefs.Save();
            UpdateAndPlayBGMusic();
            return;
        }
        PlayerPrefs.SetInt("MusicOff",1);
        PlayerPrefs.Save();
        UpdateAndPlayBGMusic();
        // SetCurrentMenu(menuOptions, mainMenu);
    }

    public void ToggleSFX()
    {
        if (PlayerPrefs.HasKey("SFXOff"))
        {
            PlayerPrefs.DeleteKey("SFXOff");
            PlayerPrefs.Save();
            return;
        }
        PlayerPrefs.SetInt("SFXOff",1);
        PlayerPrefs.Save();
        // SetCurrentMenu(menuOptions, mainMenu);
    }
}
