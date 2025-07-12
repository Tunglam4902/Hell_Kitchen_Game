using System.Collections.Generic;
using UnityEngine;

public class CharacterMeshChange : MonoBehaviour {
    [System.Serializable]
    public class MeshSlot {
        public MeshPart part;
        public SkinnedMeshRenderer targetRenderer;
        public List<Mesh> meshOptions;
    }

    public List<MeshSlot> meshSlots;

    public void ApplyCharacter(int bodyId, int cloakId) {
        foreach (var slot in meshSlots) {
            int index = slot.part switch {
                MeshPart.Body => bodyId,
                MeshPart.Cloak => cloakId,
                _ => 0
            };

            if (index >= 0 && index < slot.meshOptions.Count) {
                slot.targetRenderer.sharedMesh = slot.meshOptions[index];
            }
        }
    }
}
