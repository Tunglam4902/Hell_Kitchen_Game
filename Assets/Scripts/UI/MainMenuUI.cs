using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiPlayerButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button singlePlayerButton;

    private void Awake() {
        singlePlayerButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.multipPlayer = false;
            Loader.Load(Loader.Scene.LobbyScene); 
        });
        multiPlayerButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.multipPlayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        Time.timeScale = 1f; // Ensure time scale is reset when entering the main menu
    }
}
