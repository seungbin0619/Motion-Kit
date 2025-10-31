using System;
using System.Collections.Generic;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionSequence {
        public string name;
        public bool playOnAwake = false;

        [SerializeReference]
        public List<MotionTrack> tracks = new();

        public void Play() {
            foreach (var track in tracks) {
                track.Create();
            }
        }
    }
}
