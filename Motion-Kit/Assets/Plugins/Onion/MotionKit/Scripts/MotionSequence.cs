using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionSequence {
        public string name;
        public bool playOnAwake = false;

        [SerializeReference]
        public List<MotionTrack> tracks = new();
        private Sequence _sequence;

        public void Play() {
            if (_sequence.isAlive) {
                _sequence.Complete();    
            }

            _sequence = Sequence.Create();
            foreach (var track in tracks) {
                _sequence.Group(track.Create());
            }
        }
    }
}
