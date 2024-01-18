using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
public class AudioScript : MonoBehaviour
{
    private AudioSource src;

    public AudioClip jump;
    public AudioClip knockout;
    public AudioClip[] attacks;
    public AudioClip heavyattack;
    

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    public void JumpSound()
    {
        src.clip = jump;
        src.Play();
    }
    
    public void KnockoutSound()
    {
        src.clip = knockout;
        src.Play();
    }
    
    public void AttackSound()
    {
        src.clip = attacks[Random.Range(0,1)];
        src.Play();
    }
    
    public void HeavyAttackSound()
    {
        src.clip = heavyattack;
        src.Play();
    }
    
}
