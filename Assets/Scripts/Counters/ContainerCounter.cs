using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class ContainerCounter : BaseCounter {
    public event EventHandler OnPlayerGrabbedOnject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnPlayerGrabbedOnject?.Invoke(this, EventArgs.Empty);
        } else {
            // Player already has a KitchenObject
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                // Player is carrying a plate
                if (plateKitchenObject.TryAddIngredient(kitchenObjectSO)) {
                    // Successfully added ingredient to the plate
                    InteractLogicServerRpc();
                }
            } else {
                // Player is carrying something else, not a plate
                Debug.Log("Player is carrying something else.");
            }
        } 
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabbedOnject?.Invoke(this, EventArgs.Empty);
    }
}
