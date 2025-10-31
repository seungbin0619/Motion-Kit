using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public class MotionAnimator : MonoBehaviour {
        public List<MotionSequence> sequences = new();

        void Awake() {
            foreach (var sequence in sequences) {
                if (sequence.playOnAwake) {
                    sequence.Play();
                }
            }
        }
    }
}
