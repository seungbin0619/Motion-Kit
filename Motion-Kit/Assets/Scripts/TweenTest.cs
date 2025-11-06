using Onion.MotionKit;
using UnityEngine;

[RequireComponent(typeof(MotionAnimator))]
public class TweenTest : MonoBehaviour {
    private MotionAnimator animator;

    private void Awake() {
        animator = GetComponent<MotionAnimator>();
    }

    private void Play() {
        
    }
}