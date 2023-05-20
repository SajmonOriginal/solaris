using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public AudioSource soundSource;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(AudioClip clip)
    {
        if (instance == null || clip == null)
            return;

        instance.soundSource.PlayOneShot(clip);
    }
}

