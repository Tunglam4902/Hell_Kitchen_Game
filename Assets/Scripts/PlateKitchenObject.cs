using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private Transform plateTopPoint;
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;
    private List<GameObject> ingredientVisualList = new List<GameObject>();
    protected override void Awake() {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
        ingredientVisualList = new List<GameObject>();
    }
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        } else {
            AddIngredientServerRpc(
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO)
            );
            
            return true;    
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);

        if (kitchenObjectSO.prefab != null) {
            Transform ingredientTransform = Instantiate(kitchenObjectSO.prefab, plateTopPoint);
            RegisterIngredientVisual(ingredientTransform.gameObject);
            ingredientTransform.localPosition = Vector3.zero;
            ingredientVisualList.Add(ingredientTransform.gameObject);
        } else {
            Debug.LogWarning("KitchenObjectSO prefab is null for " + kitchenObjectSO.objectName);
        }

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            kitchenObjectSO = kitchenObjectSO
        });
    }
    public void RegisterIngredientVisual(GameObject visual) {
        ingredientVisualList.Add(visual);
    }

    public void ClearIngredientVisuals() {
        foreach (GameObject visual in ingredientVisualList) {
            if (visual != null) Destroy(visual);
        }
        ingredientVisualList.Clear();
    }
    public List<KitchenObjectSO> GetIngredientList() {
        return kitchenObjectSOList;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}
