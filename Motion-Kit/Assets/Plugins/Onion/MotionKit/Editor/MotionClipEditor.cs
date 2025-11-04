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

            while (iter.NextVisible(false)) {
                var propertyField = new PropertyField(iter);
                
                root.Add(propertyField);
            }

            return root;
        }
    }
}