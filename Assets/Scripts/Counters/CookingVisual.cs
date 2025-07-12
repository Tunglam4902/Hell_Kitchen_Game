using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingVisual : MonoBehaviour
{

    [SerializeField] private StoveCounter cookingCounter;
    [SerializeField] private GameObject cookingGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start() {
        cookingCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        bool showVisual = e.state == StoveCounter.State.Cooking || e.state == StoveCounter.State.Cooked;
        cookingGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
