using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {

    [Serializable]
    public struct RecipeVisualPair {
        public RecipeSO recipeSO;
        public GameObject completeDishPrefab;
    }

    [SerializeField] private Transform plateTopPoint;
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<RecipeVisualPair> recipeVisualPairs;

    private GameObject currentDishVisual;


    private void Awake() {
        if (plateKitchenObject == null) {
            
            plateKitchenObject = GetComponent<PlateKitchenObject>();
        }

        if (plateKitchenObject == null) {
            Debug.LogError("PlateCompleteVisual: plateKitchenObject is NULL on " + gameObject.name, this);
            return;
        }

        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        RecipeSO matchedRecipe = RecipeManager.Instance.GetMatchingRecipe(plateKitchenObject.GetIngredientList());

        if (matchedRecipe != null) {
            // Clear old dish visual if any
            plateKitchenObject.ClearIngredientVisuals();
            if (currentDishVisual != null) {
                Destroy(currentDishVisual);
                currentDishVisual = null;
            }

            // Find the correct prefab to spawn
            foreach (var pair in recipeVisualPairs) {
                if (pair.recipeSO == matchedRecipe) {
                    currentDishVisual = Instantiate(pair.completeDishPrefab,plateTopPoint);
                    currentDishVisual.transform.SetParent(plateTopPoint);
                }
            }
        }
    }
}
