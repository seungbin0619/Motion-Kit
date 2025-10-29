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

        [SerializeField]
        private VisualTreeAsset _sequenceTemplate;

        [SerializeField]
        private VisualTreeAsset _trackTemplate;

        private MotionAnimator _animator;
        private DropdownField _dropdown;
        
        private MotionSequenceView _sequenceView;
        private SerializedProperty _serializedSequenceProperty;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = _template != null 
                ? _template.CloneTree() 
                : new(); 

            _animator = (MotionAnimator)target;
            _serializedSequenceProperty = serializedObject.FindProperty("sequences");

            root.Add(CreateDropdown());
            root.Add(_sequenceView = new(_sequenceTemplate, _trackTemplate));

            SelectSequence(0);

            return root;
        }

        private VisualElement CreateDropdown() {
            _dropdown = new() { label = "Sequence" };
            _dropdown.RegisterCallback<FocusInEvent>(evt => RefreshDropdown());
            _dropdown.RegisterValueChangedCallback(OnSequenceChanged);

            RefreshDropdown();

            return _dropdown;
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

                serializedObject.Update();

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
                _sequenceView.style.display = DisplayStyle.None;
                return;
            }

            _dropdown.SetValueWithoutNotify(_dropdown.choices[index]);
            _sequenceView.SetSequence(_serializedSequenceProperty.GetArrayElementAtIndex(index));
        }
    }
}