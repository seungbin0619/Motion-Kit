using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomEditor(typeof(MotionClip), true)]
    public class MotionClipEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            var root = new VisualElement();

            var iter = serializedObject.GetIterator();
            if (!iter.NextVisible(true)) {
                return root;
            }

            PropertyField valueField = null;
            while (iter.NextVisible(false)) {
                var propertyField = new PropertyField(iter);
                if (iter.name == "value") {
                    valueField = propertyField;
                    continue;
                }
                
                root.Add(propertyField);
            }

            if (valueField != null) {
                valueField.style.marginTop = 8;
                root.Add(valueField);
            }

            return root;
        }
    }
}