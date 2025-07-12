using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter, IHasProgress {

    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
  
    public event EventHandler OnCut; 

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
        } else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc() {
        cuttingProgress = 0;

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        float progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax;

        InteractLogicPlaceObjectOnCounterClientRpc(progressNormalized);
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc(float progressNormalized) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = progressNormalized
        });
    }

    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) return;

        if (HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            CutObjectServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc() {
        if (!HasKitchenObject()) return;

        KitchenObjectSO currentSO = GetKitchenObject().GetKitchenObjectSO();
        CuttingRecipeSO recipe = GetCuttingRecipeSOWithInput(currentSO);
        if (recipe == null) return;

        cuttingProgress++;

        float normalized = (float)cuttingProgress / recipe.cuttingProgressMax;

        CutObjectClientRpc(normalized);

        if (cuttingProgress >= recipe.cuttingProgressMax) {
            KitchenObjectSO output = recipe.output;

            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObject(output, this);

            cuttingProgress = 0;

            CuttingRecipeSO newRecipe = GetCuttingRecipeSOWithInput(output);
            float newNormalized = newRecipe != null ? 0f : 1f;

            OnProgressChangedClientRpc(newNormalized);
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc(float progressNormalized) {
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = progressNormalized
        });
    }

    [ClientRpc]
    private void OnProgressChangedClientRpc(float normalized) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = normalized
        });
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetCuttingRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO recipe in cuttingRecipeSOArray) {
            if (recipe.input == inputKitchenObjectSO) return recipe;
        }
        return null;
    }
}
