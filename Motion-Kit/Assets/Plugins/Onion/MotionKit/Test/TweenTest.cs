using Onion.MotionKit;
using Sirenix.OdinInspector;
using UnityEngine;

public class TweenTest : MonoBehaviour {
    [SerializeField]
    MotionAnimator animator;
    
    [Button("Play")]
    void Play() {
        animator[0].Play();
    }
}