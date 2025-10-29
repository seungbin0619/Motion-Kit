using UnityEditor;
using UnityEditor.UIElements;
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
            VisualElement root = _template != null 
                ? _template.CloneTree() 
                : new(); 

            _animator = (MotionAnimator)target;

            root.Add(CreateDropdown());
            SelectSequence(0);

            return root;
        }

        private VisualElement CreateDropdown() {
            var column = new VisualElement();
            column.AddToClassList("sequence-container");

            var label = new Label("Sequence");
            label.AddToClassList("sequence-label");

            _dropdown = new();
            _dropdown.AddToClassList("sequence-dropdown");
            _dropdown.RegisterCallback<FocusInEvent>(evt => RefreshDropdown());
            _dropdown.RegisterValueChangedCallback(OnSequenceChanged);

            RefreshDropdown();

            column.Add(label);
            column.Add(_dropdown);

            return column;
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
            SelectSequence(index);
        }

        private void RefreshDropdown() {
            _dropdown.choices.Clear();

            foreach (var sequence in _animator.sequences) {
                _dropdown.choices.Add(sequence.name);
            }
            _dropdown.choices.Add(NewSequence);
        }

        private void SelectSequence(int index) {
            if (index < 0 || index >= _animator.sequences.Count) {
                return;
            }

            _dropdown.SetValueWithoutNotify(_dropdown.choices[index]);
            var sequence = _animator.sequences[index];
            
            // draw sequence view
        }
    }
}