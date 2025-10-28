using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public class MotionAnimator : MonoBehaviour {
        [SerializeField]
        private List<Component> _bindings;

        [SerializeField]
        private List<MotionSequence> _sequences;

        public TweenSettings<float> settings;


        [SerializeReference]
        public List<MotionTrack> tracks = new()
        {
            new MotionTrack<Color>(),
            new MotionTrack<float>(),
            new MotionTrack<Vector2>(),
            new MotionTrack<Vector3>(),
        };
    }
}
