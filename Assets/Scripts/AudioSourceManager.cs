using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    public static AudioSourceManager instance;
    public AudioSource audioSourceEffect;
    public AudioSource audioSourceBG;
    public AudioClip[] soundEffectsMan;
    public AudioClip[] soundEffectsWoman;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        audioSourceBG.Play();
    }

    public void PlaySelectedSoundEffect(int index)
    {
        if (index == 0)
        {
            audioSourceEffect.clip = soundEffectsWoman[Random.Range(0, 2)];
        }

        else
        {
            audioSourceEffect.clip = soundEffectsMan[Random.Range(0, 2)];
        }

        audioSourceEffect.Play();
    }

    public void PlayLumberingSoundEffect(int index)
    {
        if (index == 0)
        {
            audioSourceEffect.clip = soundEffectsWoman[2];
        }

        else
        {
            audioSourceEffect.clip = soundEffectsMan[2];
        }
        audioSourceEffect.Play();
    }
}
