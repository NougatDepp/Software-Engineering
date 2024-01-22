using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    
    public AudioClip[] attacks;
    public AudioClip knockout;
    public AudioClip buttonBack;
    public AudioClip buttonSelect;
    public AudioClip gameStart;
    public AudioClip clickSound;
    
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

    public void SetGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}

