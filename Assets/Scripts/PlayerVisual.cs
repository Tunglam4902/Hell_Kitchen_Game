using UnityEngine;

public class PlayerVisual : MonoBehaviour {
    [SerializeField] private CharacterMeshChange characterMeshChange;


    public void SetCharacterVisual(int bodyId, int cloakId) {
        characterMeshChange.ApplyCharacter(bodyId, cloakId);
    }
}
