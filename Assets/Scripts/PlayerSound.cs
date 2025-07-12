using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{

    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = 0.1f;
    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f) {
            footstepTimer = footstepTimerMax;

            if (player.IsRunning()) {
                float volume = 0.5f;
                SoundManager.Instance.PlayFootstepSound(player.transform.position, volume);

            }
        }
    }
}
