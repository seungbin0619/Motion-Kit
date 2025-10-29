using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomEditor(typeof(MotionAnimator))]
    public class MotionAnimatorEditor : UnityEditor.Editor {
        const string NewSequence = "New sequence...";

        [SerializeField]
        private VisualTreeAsset _template;

        private MotionAnimator _animator;
        private DropdownField _dropdown;
        
        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();
            _animator = (MotionAnimator)target;

            _dropdown = new();
            _dropdown.RegisterValueChangedCallback(OnSequenceChanged);

            RefreshDropdown();

            root.Add(_dropdown);

            return root;
        }

        private void OnSequenceChanged(ChangeEvent<string> evt) {
            var name = evt.newValue;
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            if (name.Equals(NewSequence)) {
                Undo.RecordObject(_animator, "Add Motion Sequence");

                name = $"Sequence {_animator.sequences.Count}";
                var sequence = new MotionSequence() { name = name };
                _animator.sequences.Add(sequence);

                EditorUtility.SetDirty(_animator);
                RefreshDropdown();
            }

            var index = _dropdown.choices.IndexOf(name);
            _dropdown.SetValueWithoutNotify(_dropdown.choices[index]);

            Debug.Log($"Selected sequence: {name}");
        }

        private void RefreshDropdown() {
            _dropdown.choices.Clear();

            foreach (var sequence in _animator.sequences) {
                _dropdown.choices.Add(sequence.name);
            }
            _dropdown.choices.Add(NewSequence);
        }
    }
}