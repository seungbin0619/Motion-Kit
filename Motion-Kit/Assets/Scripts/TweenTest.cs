using Onion.MotionKit;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MotionAnimator))]
public class TweenTest : MonoBehaviour {
    private MotionAnimator animator;

    private void Awake() {
        animator = GetComponent<MotionAnimator>();
    }

    [Button("Play", ButtonSizes.Large)]
    private void Play() {
        animator.Play(0);
    }

    [Button("Pause", ButtonSizes.Large)]
    private void Pause() {
        animator.Pause(0);
    }

    [Button("Stop", ButtonSizes.Large)]
    private void Stop() {
        animator.Stop(0);
    }

    [Button("Complete", ButtonSizes.Large)]
    private void Complete() {
        animator.Complete(0);
    }

    public void TestDebug(int value) {
        Debug.Log($"TestDebug called with value: {value}");
    }
}