using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public sealed class MotionAnimator : MonoBehaviour {
        private IDictionary<string, MotionSequence> _sequenceMap;
        public List<MotionSequence> sequences = new();

        public MotionSequence this[int index] => index >= 0 && index < sequences.Count 
            ? sequences[index] 
            : null;
            
        public MotionSequence this[string name] {
            get {
                InitializeSequenceMap();
                
                return _sequenceMap.TryGetValue(name, out var sequence) ? sequence : null;
            }
        }

        void Awake() {
            foreach (var sequence in sequences) {
                if (sequence.playOnAwake) {
                    sequence.Play();
                }
            }
        }

        private void InitializeSequenceMap() {
            if (_sequenceMap != null) {
                return;
            }

            _sequenceMap = new Dictionary<string, MotionSequence>();
            foreach (var sequence in sequences) {
                if (_sequenceMap.ContainsKey(sequence.name)) {
                    Debug.LogWarning($"[MotionAnimator] Duplicate sequence name detected: {sequence.name}");
                    continue;
                }
                
                _sequenceMap[sequence.name] = sequence;
            }
        }
    }
}
