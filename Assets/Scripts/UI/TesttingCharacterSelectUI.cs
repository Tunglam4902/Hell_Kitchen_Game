using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using UnityEngine.UI;

public class TesttingCharacterSelectUI : MonoBehaviour
{

    [SerializeField] private Button readyButton;

    private void Awake() {
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
} 
