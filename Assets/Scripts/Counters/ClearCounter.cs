using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // No KitchenObject here
            if (player.HasKitchenObject()) {
                // Player carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                // Nothing
            }
        } else {
            // Already have kitchen object here
            if (player.HasKitchenObject()) {
                // Player carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } else {
                    // Player is carrying something else, not a plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        // Counter has a plate, add the ingredient to the plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
                    } else {
                        // Player is carrying something else, not a plate and counter doesn't have a plate
                        Debug.Log("Player is carrying something else and counter doesn't have a plate.");
                    }
                }
            } else { 
                // Not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
