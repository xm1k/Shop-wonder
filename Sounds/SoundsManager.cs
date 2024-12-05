using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public AudioClip[] sounds;
    public SoundArrays[] randSound;

    private Camera player;
    private AudioSource DestroySoundsAudioSource;

    private void Awake()
    {
        player = FindAnyObjectByType<Camera>();
        DestroySoundsAudioSource = GameObject.FindGameObjectWithTag("DestroySoundsManager")?.GetComponent<AudioSource>();
    }
    private AudioSource audioSrc => GetComponent<AudioSource>();
    public void PlaySound(int i, float volume = 1f, bool random =false,bool destroyed=false,float p1=0.9f, float p2 = 1.2f)
    {
        AudioClip clip = random ? randSound[i].soundArray[UnityEngine.Random.Range(0, randSound[i].soundArray.Length)] : sounds[i];
        audioSrc.pitch = UnityEngine.Random.Range(p1, p2);

        
        if (destroyed)
        {
            DestroySoundsAudioSource.volume = volume;
            DestroySoundsAudioSource.clip = clip;
            DestroySoundsAudioSource.Play();
        }
        else
            audioSrc.PlayOneShot(clip, volume);
    }

    public void PlaySoundAnim(int i)
    {
        AudioClip clip = sounds[i];
        audioSrc.PlayOneShot(clip);
    }
}

[Serializable]
public class SoundArrays
{
    public AudioClip[] soundArray;
}