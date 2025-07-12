using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour {

    public const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "sound_effect_volume";

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;


    private float soundEffectVolume = 1f;

    private void Awake() {
        Instance = this;
        soundEffectVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, 1f);
    }
    private void Start() {
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;

        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;

        Player.OnAnyPlayerPickedSomething += Player_OnPickUpSomething;

        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;

        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickUpSomething(object sender, System.EventArgs e) {
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliveryFailed, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumn = 1f) {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumn);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumnMultiplier = 1f) {
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volumnMultiplier * soundEffectVolume);
        }
    }

    public void PlayFootstepSound(Vector3 position, float volume) {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }

    public void PlayCountdownSound() {
        PlaySound(audioClipRefsSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position) {
        PlaySound(audioClipRefsSO.warning, position);
    }
    public void UpVolume() {
        soundEffectVolume += .1f;
        if (soundEffectVolume > 1f) {
            soundEffectVolume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, soundEffectVolume);
        PlayerPrefs.Save();
    }
    public void DownVolume() {
        soundEffectVolume -= .1f;
        if (soundEffectVolume < 0f) {
            soundEffectVolume = 1f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, soundEffectVolume);
        PlayerPrefs.Save();
    }
    public float GetSoundEffectVolume() {
        return soundEffectVolume;
    }
}
 