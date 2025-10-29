using UnityEditor;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceView : VisualElement {
        private SerializedProperty _sequenceProperty;

        public MotionSequenceView(VisualTreeAsset template) {
            if (template != null) {
                Add(template.CloneTree());
            }
        }

        private void Repaint() {
            _sequenceProperty.serializedObject.Update();
        }
    }
}