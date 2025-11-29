using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [AddComponentMenu("Onion/MotionKit/Motion Animator")]
    public sealed class MotionAnimator : MonoBehaviour, IEnumerable<MotionSequence> {
        private IDictionary<string, MotionSequence> _sequenceMap;

        [SerializeField]
        private List<MotionSequence> sequences = new();
        public int Count => sequences.Count;

        public MotionSequence this[int index] {
            get {
                Debug.Assert(index >= 0 && index < sequences.Count, $"[MotionAnimator] Index out of range: {index}");
                
                return sequences[index];
            }
        }
            
        public MotionSequence this[string name] {
            get {
                InitializeSequenceMap();
                Debug.Assert(_sequenceMap.ContainsKey(name), $"[MotionAnimator] No sequence found with name: {name}");
                
                return _sequenceMap[name];
            }
        }

        void Awake() {
            InitializeSequenceMap();

            foreach (var sequence in sequences) {
                if (sequence.playOnAwake) {
                    sequence.Play();
                }
            }
        }

        public void Play(int index) => this[index]?.Play();
        public void Play(string name) => this[name]?.Play();
        public void Pause(int index) => this[index]?.Pause();
        public void Pause(string name) => this[name]?.Pause();
        public void Stop(int index) => this[index]?.Stop();
        public void Stop(string name) => this[name]?.Stop();
        public void Stop(int index, bool complete) => this[index]?.Stop(complete);
        public void Stop(string name, bool complete) => this[name]?.Stop(complete);
        public void Complete(int index) => this[index]?.Complete();
        public void Complete(string name) => this[name]?.Complete();

        private void InitializeSequenceMap() {
            if (_sequenceMap != null && _sequenceMap.Count == sequences.Count) {
                return;
            }

            _sequenceMap ??= new Dictionary<string, MotionSequence>();
            _sequenceMap.Clear();

            foreach (var sequence in sequences) {
                if (_sequenceMap.ContainsKey(sequence.name)) {
                    Debug.LogWarning($"[MotionAnimator] Duplicate sequence name detected: {sequence.name}");
                    continue;
                }
                
                _sequenceMap[sequence.name] = sequence;
            }
        }

        public IEnumerator<MotionSequence> GetEnumerator() {
            foreach (var sequence in sequences) {
                yield return sequence;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#if UNITY_EDITOR
        public void Add(MotionSequence sequence) {
            sequences.Add(sequence);
            
            _sequenceMap?.Clear();
        }

        void OnValidate() {
            foreach (var sequence in sequences) {
                sequence.Validate();
            }  
        }
#endif
    }
}
