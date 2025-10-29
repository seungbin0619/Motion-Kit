using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public class MotionAnimator : MonoBehaviour {
        public List<MotionSequence> sequences = new();
    }
}
