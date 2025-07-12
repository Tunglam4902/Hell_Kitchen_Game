using UnityEngine;
using UnityEngine.UI;

public class CharacterMeshChangeUI : MonoBehaviour {
    [SerializeField] private Button cycleBodyButton;
    [SerializeField] private Button cycleCloakButton;

    private void Awake() {
        cycleBodyButton.onClick.AddListener(() => {
            var pd = KitchenGameMultiplayer.Instance.GetPlayerData();
            KitchenGameMultiplayer.Instance.ChangePlayerMeshPart(MeshPart.Body, (pd.bodyId + 1) % 4);
        });

        cycleCloakButton.onClick.AddListener(() => {
            var pd = KitchenGameMultiplayer.Instance.GetPlayerData();
            KitchenGameMultiplayer.Instance.ChangePlayerMeshPart(MeshPart.Cloak, (pd.cloakId + 1) % 2);
        });
    }
}
