using System.Linq;
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
        private PopupField<string> _popup;
        
        private MotionSequenceView _sequenceView;
        private SerializedProperty _serializedSequenceProperty;

        private VisualElement _parentScroller;
        private bool _isWide = false;
        private bool _hasScroller = false;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = _template != null 
                ? _template.CloneTree() 
                : new(); 

            _animator = (MotionAnimator)target;
            _serializedSequenceProperty = serializedObject.FindProperty("sequences");
            

            root.Add(CreateDropdown());
            root.Add(_sequenceView = new(_sequenceTemplate, _trackTemplate));
            root.RegisterCallback<GeometryChangedEvent>(OnInspectorResized);

            SelectSequence(0);

            return root;
        }

        private void OnInspectorResized(GeometryChangedEvent evt) {
            if (evt.target is not VisualElement ve) return;

            _parentScroller ??= ve.GetFirstAncestorOfType<ScrollView>()?
                .Q("unity-content-and-vertical-scroll-container")
                .Children().Last();

            if (_parentScroller is Scroller && _parentScroller.contentRect.width > 0) {
                ve.AddToClassList("sequence__with-scroller");
                _hasScroller = true;
            } else if (_hasScroller) {
                ve.RemoveFromClassList("sequence__with-scroller");
                _hasScroller = false;
            }

            if (evt.newRect.width >= 335) {
                _isWide = true;
                ve.AddToClassList("sequence__wide-inspector");
            } else if (_isWide) {
                ve.RemoveFromClassList("sequence__wide-inspector");
                _isWide = false;
            }
        }

        private VisualElement CreateDropdown() {
            _popup = new() { label = "Sequence" };
            _popup.AddToClassList("sequence-popup");
            _popup.RegisterCallback<FocusInEvent>(evt => RefreshDropdown());
            _popup.RegisterValueChangedCallback(OnSequenceChanged);

            RefreshDropdown();

            return _popup;
        }

        private void OnSequenceChanged(ChangeEvent<string> evt) {
            var name = evt.newValue;
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            if (name.Equals(NewSequence)) {
                Undo.RecordObject(_animator, "Add Motion Sequence");

                name = $"Sequence {_animator.Count}";
                var sequence = new MotionSequence() { name = name };
                _animator.Add(sequence);

                serializedObject.Update();

                EditorUtility.SetDirty(_animator);
                RefreshDropdown();
            }

            var index = _popup.choices.IndexOf(name);
            SelectSequence(index);
        }

        private void RefreshDropdown() {
            _popup.choices.Clear();

            foreach (var sequence in _animator) {
                _popup.choices.Add(sequence.name);
            }
            _popup.choices.Add(NewSequence);
        }

        private void SelectSequence(int index) {
            if (index < 0 || index >= _animator.Count) {
                _sequenceView.style.display = DisplayStyle.None;
                return;
            }

            _popup.SetValueWithoutNotify(_popup.choices[index]);
            _sequenceView.SetSequence(_serializedSequenceProperty.GetArrayElementAtIndex(index));
        }
    }
}