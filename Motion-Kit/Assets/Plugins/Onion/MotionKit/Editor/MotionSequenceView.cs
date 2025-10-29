using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceView : VisualElement {
        private SerializedProperty _sequenceProperty;

        private readonly VisualElement _rootContainer;
        private readonly VisualElement _trackContainer;
        private readonly PropertyField _nameField;

        public MotionSequenceView(VisualTreeAsset template) {
            if (template == null) return;

            Add(template.CloneTree());

            _rootContainer = this.Q<VisualElement>("root-container");
            _trackContainer = this.Q<VisualElement>("track-container");

            _nameField = new() { label = "Name" };
            _rootContainer.Insert(0, _nameField);

            SetSequence(null);
        }

        private void Repaint() {
            _trackContainer.Clear();

            if (_sequenceProperty == null) {
                style.display = DisplayStyle.None;
                return;
            }

            style.display = DisplayStyle.Flex;
            _sequenceProperty.serializedObject.Update();

        }

        public void SetSequence(SerializedProperty sequenceProperty) {
            _nameField.Unbind();
            _sequenceProperty = sequenceProperty;
            
            if (_sequenceProperty != null) {
                _nameField.BindProperty(_sequenceProperty.FindPropertyRelative("name"));
            }
            
            Repaint();
        }
    }
}