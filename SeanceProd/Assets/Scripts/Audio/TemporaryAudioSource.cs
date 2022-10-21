using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryAudioSource : MonoBehaviour
{
    public AudioSource _as;
    public bool _isInitialized = false;

    public void Init(AudioClip clip, float volume = 1)
    {
        _as = GetComponent<AudioSource>();
        _as.clip = clip;
        _as.volume = volume;
        _as.Play();

        _isInitialized = true;
    }

    void Update()
    {
        if(_isInitialized && !_as.isPlaying)
        {
            DestroyImmediate(gameObject);
        }
    }
}
