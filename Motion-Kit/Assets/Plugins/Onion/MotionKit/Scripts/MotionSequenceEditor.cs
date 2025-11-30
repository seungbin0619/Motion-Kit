#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Onion.MotionKit {
    public partial class MotionSequence {
        [InitializeOnLoad]
        private static class MotionSequenceBaker {
            private static readonly HashSet<MotionSequence> _bakeSeqeuences = new();

            public static void AddToBakeList(MotionSequence sequence) {
                _bakeSeqeuences.Add(sequence);
            }

            static MotionSequenceBaker() {
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                EditorSceneManager.sceneSaving += OnSceneSaving;
            }

            private static void OnPlayModeStateChanged(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingEditMode) {
                    BakeAll();
                }
            }

            private static void OnSceneSaving(Scene scene, string path) {
                BakeAll();
            }

            private static void BakeAll() {
                foreach (var sequence in _bakeSeqeuences) {
                    sequence.Bake();
                }

                _bakeSeqeuences.Clear();
            }
        }

        private readonly Dictionary<int, List<string>> _usedProperties = new();
        private bool _isDirty = false;

        public void Validate() {
            _isDirty = true;

            MotionSequenceBaker.AddToBakeList(this);
        }

        private void Bake() {
            if (!_isDirty) return;
            _isDirty = false;

            _usedProperties.Clear();
            foreach (var track in tracks) {
                if (track.clip == null || track.target == null) {
                    track.readyOnPlay = false;
                    continue;    
                }

                int hash = track.target.GetInstanceID();
                
                track.readyOnPlay = 
                    !_usedProperties.TryGetValue(hash, out var keys) ||
                    !keys.Any(k => IsConflicKey(k, track.clip.propertyKey));

                if (track.readyOnPlay) {
                    if (!_usedProperties.ContainsKey(hash)) {
                        _usedProperties[hash] = new();
                    }

                    _usedProperties[hash].Add(track.clip.propertyKey);
                }
            }
        }

        private bool IsConflicKey(string a, string b) {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            
            if (a == b) return true;
            if (a.StartsWith(b + ".")) return true;
            if (b.StartsWith(a + ".")) return true;

            return false;
        }
    }
}
#endif