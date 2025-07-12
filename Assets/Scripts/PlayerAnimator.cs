using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour {

    private const string IS_RUNNING = "IsRunning";

    private const string PICKUP_TRIGGER = "Pickup";

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        
        player.OnKitchenObjectChanged += Player_OnKitchenObjectChanged;
    }
    private void Player_OnKitchenObjectChanged(object sender, System.EventArgs e) {
        animator.SetTrigger(PICKUP_TRIGGER);
    }
    private void Update() {
        if (!IsOwner) {
            return;
        }
        animator.SetBool(IS_RUNNING, player.IsRunning());
    }

}
