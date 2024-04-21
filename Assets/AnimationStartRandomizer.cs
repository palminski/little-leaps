using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStartRandomizer : MonoBehaviour
{

    private Animator animator;
    [SerializeField]
    private float maxOffset = 1;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator) return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float randomStart = Random.Range(0f, maxOffset);
        animator.Play(state.fullPathHash, -1, randomStart);
    }


}
