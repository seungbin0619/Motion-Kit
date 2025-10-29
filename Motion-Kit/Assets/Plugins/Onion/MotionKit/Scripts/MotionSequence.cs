using System;
using System.Collections.Generic;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionSequence {
        public string name;

        [SerializeReference]
        public List<MotionTrack> tracks = new() {
            new MotionTrack<float>(),
            new MotionTrack<Vector3>(),
            new MotionTrack<Color>()
        };
    }
}
