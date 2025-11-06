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

            bool hasBufferedChain = false;
            float accumulatedTime = 0f;
            float maxGroupTime = 0f;
            
            _sequence = Sequence.Create();

            foreach (var track in tracks) {
                if (!track.isValid) continue;

                var tween = track.Create();
                if (track.runIndependently) {
                    if (track.mode == TrackMode.Chain) {
                        hasBufferedChain = true;
                        accumulatedTime += maxGroupTime;
                        maxGroupTime = 0f;
                    }

                    if (accumulatedTime == 0f) continue;
                    Tween.Delay(accumulatedTime).Chain(tween);
                    continue;
                }

                if (!hasBufferedChain && track.mode == TrackMode.Group) {
                    _sequence.Group(tween);
                }
                else {
                    accumulatedTime += maxGroupTime;
                    maxGroupTime = 0f;

                    _sequence.Chain(tween);
                }

                maxGroupTime = Mathf.Max(maxGroupTime, tween.durationTotal);
            }
        }
    }
}
