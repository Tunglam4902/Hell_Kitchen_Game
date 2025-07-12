using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour {

    public static OptionUI Instance { get; private set; }

    [SerializeField] private Button upSoundVolumeButton;
    [SerializeField] private Button dowmSoundVolumeButton;
    [SerializeField] private Button upMusicVolumeButton;
    [SerializeField] private Button dowmMusicVolumeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button cuttingButton;
    [SerializeField] private Button pickUpButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamePadInteractButton;
    [SerializeField] private Button gamePadInteractAlternateButton;
    [SerializeField] private Button gamePadPauseButton;
    [SerializeField] private TextMeshProUGUI soundVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI cuttingText;
    [SerializeField] private TextMeshProUGUI pickUpText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamePadInteractText;
    [SerializeField] private TextMeshProUGUI gamePadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI gamePadPauseText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction;

    private void Awake() {
        Instance = this;
        upSoundVolumeButton.onClick.AddListener(() => {
            SoundManager.Instance.UpVolume();
            UpdateVisual();
        });
        dowmSoundVolumeButton.onClick.AddListener(() => {
            SoundManager.Instance.DownVolume();
            UpdateVisual();
        });
        upMusicVolumeButton.onClick.AddListener(() => {
            MusicManager.Instance.UpVolume();
            UpdateVisual();
        });
        dowmMusicVolumeButton.onClick.AddListener(() => {
            MusicManager.Instance.DownVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => {
            Hide();
            onCloseButtonAction();
        });
        moveUpButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Up);
        });
        moveDownButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Down);
        });
        moveLeftButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Left);
        });
        moveRightButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Move_Right);
        });
        cuttingButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.InteractAlternate);
        });
        pickUpButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Interact);
        });
        pauseButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Pause);
        });
        gamePadInteractButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Gamepad_Interact);
        });
        gamePadInteractAlternateButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Gamepad_InteractAlternate);
        });
        gamePadPauseButton.onClick.AddListener(() => {
            RebindBinding(GameInput.Binding.Gamepad_Pause);
        });

    }
    private void Start() {
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnGameUnpaused;
        UpdateVisual();
        Hide();
        HidePressToRebindKey();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void UpdateVisual() {
        soundVolumeText.text = "" + Mathf.Round(SoundManager.Instance.GetSoundEffectVolume() * 10f);
        musicVolumeText.text = "" + Mathf.Round(MusicManager.Instance.GetMusicEffectVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        cuttingText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pickUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamePadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamePadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamePadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }
    public void Show(Action onCloseButtonAction) {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);

        closeButton.Select();
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey() {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding) {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
