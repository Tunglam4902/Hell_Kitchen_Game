using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour {
    public static RecipeManager Instance { get; private set; }

    public List<RecipeSO> recipeList;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public RecipeSO GetMatchingRecipe(List<KitchenObjectSO> currentIngredients) {
        foreach (RecipeSO recipe in recipeList) {
            if (IsMatching(recipe, currentIngredients)) {
                return recipe;
            }
        }
        return null;
    }

    private bool IsMatching(RecipeSO recipe, List<KitchenObjectSO> ingredients) {
        if (recipe.requiredIngredients.Count != ingredients.Count) return false;

        foreach (var ingredient in recipe.requiredIngredients) {
            if (!ingredients.Contains(ingredient))
                return false;
        }
        return true;
    }
}
