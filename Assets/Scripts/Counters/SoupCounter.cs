using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoupCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedOnject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
           //nothing happen

        } else {
            // Player already has a KitchenObject
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                // Player is carrying a plate
                if (plateKitchenObject.TryAddIngredient(kitchenObjectSO)) {
                    // Successfully added ingredient to the plate
                    OnPlayerGrabbedOnject?.Invoke(this, EventArgs.Empty);
                }
            } else {
                // Player is carrying something else, not a plate
                Debug.Log("Player is carrying something else.");
            }
        }
    }
}
