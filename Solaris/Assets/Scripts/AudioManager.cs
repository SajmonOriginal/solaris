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
        // Zajištění jedinečnosti instance AudioManageru při spuštění hry
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Pokud již existuje jiná instance AudioManageru, znič tuto instanci
            Destroy(gameObject);
            return;
        }

        // Zajištění, aby se tento objekt nezničil při přechodu na jiný herní úroveň
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(AudioClip clip)
    {
        // Ověření, zda instance AudioManageru existuje a zda je předán platný zvukový klip
        if (instance == null || clip == null)
            return;

        // Přehraje zvukový klip přes AudioSource instance AudioManageru
        instance.soundSource.PlayOneShot(clip);
    }
}
