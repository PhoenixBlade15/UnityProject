using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    bool inTrigger = false;

    private void Awake()
    {
        animator.enabled = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && inTrigger)
        {
            animator.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        inTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
    }
}
