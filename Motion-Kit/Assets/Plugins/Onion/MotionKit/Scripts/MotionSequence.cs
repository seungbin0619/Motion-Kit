using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    #pragma warning disable IDE1006

    [Serializable]
    public class MotionSequence {
        public string name;
        public bool playOnAwake = false;

        [SerializeReference]
        public List<MotionTrack> tracks = new();

        private Sequence _sequence;
        private readonly List<Tween> _independentTweens = new();
        private readonly List<Sequence> _independentSequences = new();

        private bool isAlive => _sequence.isAlive 
            || _independentTweens.Exists(tween => tween.isAlive)
            || _independentSequences.Exists(seq => seq.isAlive);

        private bool isPaused => isAlive && (_sequence.isPaused 
            || _independentTweens.Exists(tween => tween.isPaused)
            || _independentSequences.Exists(seq => seq.isPaused));

        public void Play() {
            if (isPaused) {
                Resume();
                return;
            }

            if (isAlive) return;

            bool hasBufferedChain = false;
            float accumulatedTime = 0f;
            float maxGroupTime = 0f;
            
            _sequence = Sequence.Create();

            _independentTweens.Clear();
            _independentSequences.Clear();

            foreach (var track in tracks) {
                if (!track.isValid) continue;

                var tween = track.Create();
                if (track.runIndependently) {
                    if (track.mode == TrackMode.Chain) {
                        hasBufferedChain = true;
                        accumulatedTime += maxGroupTime;
                        maxGroupTime = 0f;
                    }

                    if (accumulatedTime == 0f) {
                        _independentTweens.Add(tween);
                    } 
                    else {
                        _independentSequences.Add(Tween.Delay(accumulatedTime).Chain(tween));
                    }

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

        private void Resume() {
            _sequence.isPaused = false;

            foreach (var tween in _independentTweens) {
                tween.isPaused = false;
            }

            foreach (var seq in _independentSequences) {
                seq.isPaused = false;
            }
        }

        public void Pause() {
            _sequence.isPaused = true;

            foreach (var tween in _independentTweens) {
                tween.isPaused = true;
            }
            
            foreach (var seq in _independentSequences) {
                seq.isPaused = true;
            }
        }

        public void Stop(bool complete = false) {
            if (!isAlive) return;
            if (complete) {
                Complete();
                return;
            }

            _sequence.Stop();
            
            foreach (var tween in _independentTweens) tween.Stop();
            foreach (var seq in _independentSequences) seq.Stop();
        }

        public void Complete() {
            _sequence.Complete();

            foreach (var tween in _independentTweens) tween.Complete();
            foreach (var seq in _independentSequences) seq.Complete();
        }
    }
}
