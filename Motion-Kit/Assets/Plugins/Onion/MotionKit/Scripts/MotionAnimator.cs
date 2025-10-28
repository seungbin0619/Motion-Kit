using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public class MotionAnimator : MonoBehaviour {
        [SerializeField]
        private List<Object> _bindings;

        [SerializeField]
        private List<MotionSequence> _sequences;
    }
}
