using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static BGSoundManager instance;
    AudioSource myAudio;
    public bool isOn = false;

    [Header("BGM")]
    [Header("GunFight")]
    public AudioClip GunfightMainBGM;
    public AudioClip GunfightLobbyBGM;
    public AudioClip GunfightPlayBGM;
    [Header("CardMemoryGame")]
    public AudioClip cardGameMainBGM;
    public AudioClip cardGameStage1BGM;
    public AudioClip cardGameStage2BGM1;
    public AudioClip cardGameStage2BGM2;
    public AudioClip cardGameStage3BGM;
    public AudioClip cardGameStage4BGM;
    [Header("EnchantGame")]
    public AudioClip enchantGameMainBGM;
    public AudioClip enchantGamePlayBGM;
    [Header("SpaceDefence")]
    public AudioClip mainBGM;
    public AudioClip gameBGM;
    public AudioClip buildBGM;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isOn = true;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }
    public void Play()
    {
        if (myAudio != null && isOn)
        {
            myAudio.Play();
        }

    }
    public void VolumeSet(float volume)
    {
        myAudio.Pause();
        myAudio.volume = volume;
        myAudio.Play();
    }
    public void StopBGM()
    {
        if (myAudio != null)
        {
            myAudio.Stop();
        }
    }
    /// <summary>
    /// GunFight BGM
    /// </summary>
    public void PlayGunfightMainBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = GunfightMainBGM;
            myAudio.loop = true;
            myAudio.volume = 0.1f;
            myAudio.Play();
        }
    }

    public void PlayGunfightLobbyBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = GunfightLobbyBGM;
            myAudio.loop = true;
            myAudio.volume = 0.1f;
            myAudio.Play();
        }
    }

    public void PlayGunfightGameBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = GunfightPlayBGM;
            myAudio.loop = true;
            myAudio.volume = 0.1f;
            myAudio.Play();
        }
    }


    /// <summary>
    /// CardMemoryGame BGM
    /// </summary>


    public void PlayCardGameMainBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameMainBGM;
            myAudio.loop = true;
            myAudio.volume = 0.5f;
            myAudio.Play();
        }
    }


    public void PlayCardMemoryGameStage1BGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameStage1BGM;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }

    public void PlayCardMemoryGameStage2BGM1()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameStage2BGM1;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }

    public void PlayCardMemoryGameStage2BGM2()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameStage2BGM2;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }
    public void PlayCardMemoryGameStage3BGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameStage3BGM;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }
    public void PlayCardMemoryGameStage4BGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = cardGameStage4BGM;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }

    /// <summary>
    /// GunFight BGM
    /// </summary>

    public void PlayEnchantGameMainBGM()
	{
        if (myAudio != null && isOn)
        {
            myAudio.clip = enchantGameMainBGM;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }

    public void PlayEnchantGamePlayBGM()
	{
        if (myAudio != null && isOn)
        {
            myAudio.clip = enchantGamePlayBGM;
            myAudio.loop = true;
            myAudio.volume = 0.3f;
            myAudio.Play();
        }
    }

    /// <summary>
    /// SpaceDefence BGM
    /// </summary>
    public void PlaySpaceDefenceMainBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = mainBGM;
            myAudio.loop = true;
            myAudio.volume = 0.2f;
            myAudio.Play();
        }
    }


    public void PlaySpaceDefenceGameBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = gameBGM;
            myAudio.loop = true;
            myAudio.volume = 0.1f;
            myAudio.Play();
        }
    }
    public void PlaySpaceDefenceBuildBGM()
    {
        if (myAudio != null && isOn)
        {
            myAudio.clip = buildBGM;
            myAudio.loop = true;
            myAudio.volume = 0.1f;
            myAudio.Play();
        }
    }
}
