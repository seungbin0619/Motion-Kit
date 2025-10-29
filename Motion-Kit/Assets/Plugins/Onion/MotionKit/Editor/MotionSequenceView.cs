
using UnityEditor;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceView : VisualElement {
        private SerializedProperty _sequenceProperty;


        private void Repaint() {
            _sequenceProperty.serializedObject.Update();
        }
    }
}