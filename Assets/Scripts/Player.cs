using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent 
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickedSomething;

    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }
    public static Player LocalInstance {get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public event EventHandler OnKitchenObjectChanged;

    public event EventHandler OnPickUpSomething;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionsList;
    [SerializeField] private PlayerVisual playerVisual;


    private bool isRunning;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;


    private void Awake() {
        if (playerVisual == null) {
            playerVisual = GetComponentInChildren<PlayerVisual>();
            if (playerVisual == null) {
                Debug.LogError("[Player] Missing PlayerVisual reference!");
            }
        }
    }

    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

    }

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

        int playerIndex = KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId);
        transform.position = spawnPositionsList[playerIndex];

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        if (playerVisual != null) {
            playerVisual.SetCharacterVisual(playerData.bodyId, playerData.cloakId);
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }


    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId && HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) { 
            return; 
        }
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) {
            return;
        }
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }
        HandleMovement();
        HandleInteractions();
    }

    public bool IsRunning() {
        return isRunning;
    }

    private void HandleInteractions() {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
            
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) { 
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                // Has ClearCounter
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);

                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }
    private void HandleMovement() {

        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        float playerRadius = .7f;

        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance, collisionLayerMask);

        if (!canMove) {
            // Check if we can move in the X or Z direction
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = (moveDir.x < -.5f || moveDir.x > .5f)  && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance, collisionLayerMask);

            if (canMove) {
                moveDir = moveDirX;
            } else {

                // Check if we can move in the Z direction
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = (moveDir.z < -.5f || moveDir.z > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance, collisionLayerMask);

                if (canMove) {
                    moveDir = moveDirZ;
                } else {

                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isRunning = moveDir != Vector3.zero;

        if (moveDir != Vector3.zero) {
            float rotateSpeed = 80f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }
    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }
    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        OnKitchenObjectChanged?.Invoke(this, EventArgs.Empty);

        if (kitchenObject != null) {
            OnPickUpSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
        OnKitchenObjectChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject() {
        return NetworkObject;
    }
}
