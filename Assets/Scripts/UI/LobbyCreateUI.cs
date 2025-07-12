using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{

    [SerializeField] private Button backButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            Hide();
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
    }
    private void Start() {
        Hide();
    }
    public void Show() {
        gameObject.SetActive(true);
        createPublicButton.Select();
    }
    private void Hide() { 
        gameObject.SetActive(false);
    }
}
