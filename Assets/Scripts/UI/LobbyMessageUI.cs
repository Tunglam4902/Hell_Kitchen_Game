using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {

    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed; ;
        GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        Hide();
    }

    private void GameLobby_OnJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to join lobby.");
    }

    private void GameLobby_OnQuickJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Quick join failed. Please try create a lobby.");
    }

    private void GameLobby_OnJoinStarted(object sender, System.EventArgs e) {
        ShowMessage("Joining lobby...");
    }

    private void GameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to create lobby. Please try again.");
    }

    private void GameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
        ShowMessage("Creating lobby...");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to join game. Please try again.");
        } else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }


    private void ShowMessage (string message) {
        Show();
        messageText.text = message;
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
        GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
    }
}
