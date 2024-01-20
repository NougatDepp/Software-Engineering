using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public AudioClip music;

    public AudioClip[] attacks;
    public AudioClip knockout;
    
    


    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "AudioManager";
                    instance = obj.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    public AudioSource backgroundMusicSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AudioListener.volume = PlayerPrefs.GetFloat("GameVolume");
        PlaySound(music);
        
    }

    public void PlaySound(AudioClip clip)
    {
        GameObject soundObject = new GameObject("Sound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = AudioListener.volume;
        audioSource.transform.tag = "AudioSource";
        audioSource.Play();

        Destroy(soundObject, clip.length);
    }

    public void PlayBackgroundMusic(AudioClip musicClip, float volume)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
            backgroundMusicSource.clip = musicClip;
            backgroundMusicSource.volume = volume;
            backgroundMusicSource.Play();
        }
    }

    public void SetGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
        
        
    }
}

