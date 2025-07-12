using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionButton;


    private void Awake() {
        resumeButton.onClick.AddListener(() => {
            GameManager.Instance.TooglePauseGame();
        });
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionButton.onClick.AddListener(() => {
            Hide();
            OptionUI.Instance.Show(Show);
        });
    }
    private void Start() {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;
        Hide();
    }

    private void GameManager_OnLocalGameUnpaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);

        resumeButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
