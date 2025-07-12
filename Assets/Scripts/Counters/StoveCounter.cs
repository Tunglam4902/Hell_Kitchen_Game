using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress {


    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }


    public enum State {
        Idle,
        Cooking,
        Cooked,
        Burned,
    }


    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;


    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> cookingTimer = new NetworkVariable<float>(0f);
    private CookingRecipeSO cookingRecipeSO;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipeSO;


    public override void OnNetworkSpawn() {
        cookingTimer.OnValueChanged += cookingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void cookingTimer_OnValueChanged(float previousValue, float newValue) {
        float cookingTimerMax = cookingRecipeSO != null ? cookingRecipeSO.cookingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = cookingTimer.Value / cookingTimerMax
        });
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue) {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burningTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State previousState, State newState) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state.Value
        });

        if (state.Value == State.Burned || state.Value == State.Idle) {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = 0f
            });
        }
    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        if (HasKitchenObject()) {
            switch (state.Value) {
                case State.Idle:
                    break;
                case State.Cooking:
                    cookingTimer.Value += Time.deltaTime;

                    if (cookingTimer.Value > cookingRecipeSO.cookingTimerMax) {
                        // Cooked
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(cookingRecipeSO.output, this);

                        state.Value = State.Cooked;
                        burningTimer.Value = 0f;
                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                        );
                    }
                    break;
                case State.Cooked:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > burningRecipeSO.burningTimerMax) {
                        // cooked but burned
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.Burned;

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    // Player carrying something that can be Cooked
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
                    );

                }
            } else {
                // Player not carrying anything
            }
        } else {
            // There is a KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        SetStateIdleServerRpc();
                    }
                }
            } else {
                // Player is not carrying anything
               // nothing happen
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex) {
        cookingTimer.Value = 0f;
        state.Value = State.Cooking;

        SetcookingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetcookingRecipeSOClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        cookingRecipeSO = GetcookingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        CookingRecipeSO cookingRecipeSO = GetcookingRecipeSOWithInput(inputKitchenObjectSO);
        return cookingRecipeSO != null;
    }


    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CookingRecipeSO cookingRecipeSO = GetcookingRecipeSOWithInput(inputKitchenObjectSO);
        if (cookingRecipeSO != null) {
            return cookingRecipeSO.output;
        } else {
            return null;
        }
    }

    private CookingRecipeSO GetcookingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CookingRecipeSO cookingRecipeSO in cookingRecipeSOArray) {
            if (cookingRecipeSO.input == inputKitchenObjectSO) {
                return cookingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
            if (burningRecipeSO.input == inputKitchenObjectSO) {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsCooked() {
        return state.Value == State.Cooked;
    }

}