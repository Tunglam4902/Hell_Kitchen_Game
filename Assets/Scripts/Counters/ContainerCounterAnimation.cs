using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterAnimation : MonoBehaviour
{
    private const string CRATEPICKFOOD = "CratePickFood";

    [SerializeField] private ContainerCounter containerCounter;    

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        containerCounter.OnPlayerGrabbedOnject += ContainerCounter_OnPlayerGrabbedObject;
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e) {
        animator.SetTrigger(CRATEPICKFOOD);
    }
}
