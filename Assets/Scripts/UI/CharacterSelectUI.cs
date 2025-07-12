using System.Collections;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown(); // Ensure the network is shut down before loading the main menu
            GameLobby.Instance.LeaveLobby(); // Leave the lobby if in one
            Loader.Load(Loader.Scene.MainMenuScene); 
        });
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start() {
        Lobby lobby = GameLobby.Instance.GetJoinedLobby();
        lobbyNameText.text = "Lobby: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
