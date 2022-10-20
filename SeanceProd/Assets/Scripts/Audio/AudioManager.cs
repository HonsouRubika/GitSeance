using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton
    public static AudioManager Instance;

    // Audio players components.
    public AudioSource _effectsSource;
    public AudioSource _musicSource;
    public AudioSource _mjSource;

    // Random pitch adjustment range.
    public float _lowPitchRange = .95f;
    public float _highPitchRange = 1.05f;

    // For multiple effects
    public GameObject _tmpAudioSourcePrefab;

    void Awake()
    {
        #region Make Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Set AudioManager to DontDestroyOnLoad so that it won't be destroyed when reloading or change scene.
        //DontDestroyOnLoad(gameObject);
        #endregion
    }


    private void Start()
    {
        _musicSource.Play();

        PlayEffectOnTmpSource(_effectsSource.clip);
    }

    public void SetEffectsSourceFromGameObejct(GameObject go)
    {
        if (go.GetComponent<AudioSource>() != null)
            _effectsSource = go.GetComponent<AudioSource>();
    }

    public void SetEffectVolume(float volume)
    {
        _effectsSource.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }

    public void SetMJVolume(float volume)
    {
        _mjSource.volume = volume;
    }

    public void PlayEffect(AudioClip clip)
    {
        _effectsSource.clip = clip;
        _effectsSource.Play();
    }

    public void PlayEffectOnTmpSource(AudioClip clip, float volume = 1)
    {
        GameObject go = Instantiate<GameObject>(_tmpAudioSourcePrefab);
        go.GetComponent<TemporaryAudioSource>().Init(clip, volume);
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlayMJVoice(AudioClip clip)
    {
        _mjSource.clip = clip;
        _mjSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(_lowPitchRange, _highPitchRange);
        _effectsSource.pitch = randomPitch;
        _effectsSource.clip = clips[randomIndex];
        _effectsSource.Play();
    }

}
