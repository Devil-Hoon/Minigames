using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSoundManager : MonoBehaviour
{
    public static EffectSoundManager instance;
    AudioSource myAudio;
    public bool isOn = false;

    [Header("Effect")]
    [Header("GunFight")]
    public AudioClip gunShot;
    public AudioClip explosion;
    public AudioClip laser;
    [Header("CardMemoryGame")]
    public AudioClip cardMemoryGameCountDown;
    public AudioClip cardMemoryGameStageClear;
    public AudioClip cardMemoryGameCardSame;
    public AudioClip cardMemoryGameStageFail;
    [Header("EnchantGame")]
    public AudioClip enchantGameFireWork;
    public AudioClip enchantGameEarthquake;
    public AudioClip enchantGameEnchantSuccess;
    public AudioClip enchantGameEnchantFail;

    [Header("SpaceDefence")]
    public AudioClip destroy;
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
    public void Stop()
    {
        if (myAudio != null && isOn)
        {
            myAudio.Stop();
        }
    }
    /// <summary>
    /// Gunfight
    /// </summary>
    public void GunShot()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.3f;
            myAudio.PlayOneShot(gunShot);
        }
    }
    public void Explosion()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 1.0f;
            myAudio.PlayOneShot(explosion);
        }
    }
	public void Laser()
	{
		if (myAudio != null && isOn)
		{
			myAudio.volume = 1.0f;
			myAudio.PlayOneShot(laser);
		}
	}
    /// <summary>
    /// CardMemoryGame
    /// </summary>
    public void CardMemoryGameCountDown()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(cardMemoryGameCountDown);
        }
    }
    public void CardMemoryGameStageClear()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(cardMemoryGameStageClear);
        }
    }
    public void CardMemoryGameCardSame()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 1.0f;
            myAudio.PlayOneShot(cardMemoryGameCardSame);
        }
    }
    public void CardMemoryGameStageFail()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(cardMemoryGameStageFail);
        }
    }
    /// <summary>
    /// EnchantGame
    /// </summary>

    public void EnchantGameFireWork()
	{
		if (myAudio != null && isOn)
		{
            myAudio.volume = 1.0f;
            myAudio.PlayOneShot(enchantGameFireWork);
		}
	}
    public void EnchantGameEarthquake()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(enchantGameEarthquake);
        }
    }
    public void EnchantGameEnchantSuccess()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(enchantGameEnchantSuccess);
        }
    }
    public void EnchantGameEnchantFail()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.5f;
            myAudio.PlayOneShot(enchantGameEnchantFail);
        }
    }

    /// <summary>
    /// SpaceDefence
    /// </summary>
    public void SpaceDefenceEnemyDestroy()
    {
        if (myAudio != null && isOn)
        {
            myAudio.volume = 0.15f;
            myAudio.PlayOneShot(destroy);
        }
    }
}
