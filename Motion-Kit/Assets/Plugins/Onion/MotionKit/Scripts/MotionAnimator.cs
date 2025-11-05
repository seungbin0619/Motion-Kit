using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public sealed class MotionAnimator : MonoBehaviour {
        public List<MotionSequence> sequences = new();
        public MotionSequence this[int index] => sequences[index];

        void Awake() {
            foreach (var sequence in sequences) {
                if (sequence.playOnAwake) {
                    sequence.Play();
                }
            }
        }
    }
}
