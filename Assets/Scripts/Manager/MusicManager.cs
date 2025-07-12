using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public const string PLAYER_PREFS_MUSIC_VOLUME = "music_volume";
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float musicVolume = .5f;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        musicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .5f);
        audioSource.volume = musicVolume;
    }
    public void UpVolume() {
        musicVolume += .1f;
        if (musicVolume > 1f) {
            musicVolume = 0f;
        }
        audioSource.volume = musicVolume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
    }
    public void DownVolume() {
        musicVolume -= .1f;
        if (musicVolume < 0f) {
            musicVolume = 1f;
        }
        audioSource.volume = musicVolume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
    }
    public float GetMusicEffectVolume() {
        return musicVolume;
    }
}
