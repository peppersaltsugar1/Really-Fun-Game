using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource BackGroundSource;
    public AudioSource StartSound;
    public AudioSource FireSound;
    public AudioMixer audioMixer;
    public List<AudioClip> backgroundMusicList;

    // Mute ���� üũ
    public bool MasterVolumeMute = false;
    public bool BGMVolumeMute = false;
    public bool SFXVolumeMute = false;

    public float MasterVolume = 0;
    public float BGMVolume = 0;
    public float SFXVolume = 0;

    private int currentTrackIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        BackGroundSource.ignoreListenerPause = true;
        PlayMusic();  // ù Ʈ�� ���
    }

    void Update()
    {
        if (!BackGroundSource.isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % backgroundMusicList.Count;
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        if (backgroundMusicList.Count == 0) return;

        BackGroundSource.clip = backgroundMusicList[currentTrackIndex];
        BackGroundSource.Play();
    }

    // ������ ���� ����
    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;

        if (volume <= -40.0f || MasterVolumeMute)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", MasterVolume);
        }
    }

    // BGM ���� ����
    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;

        if (volume <= -40.0f || BGMVolumeMute)
        {
            audioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            audioMixer.SetFloat("BGM", volume);
        }
    }

    // SFX ���� ����
    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;

        if (volume <= -40.0f || SFXVolumeMute)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", volume);
        }
    }

    public void StartButtonSound()
    {
        StartSound.Play();
    }

    public void PlayerFireSound()
    {
        FireSound.Play();
    }
}