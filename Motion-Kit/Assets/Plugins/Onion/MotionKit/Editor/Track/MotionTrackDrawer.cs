using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomPropertyDrawer(typeof(MotionTrack), useForChildren: true)]
    public class MotionTrackDrawer : PropertyDrawer {
        [SerializeField]
        private VisualTreeAsset _template;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            if (_template == null) {
                return base.CreatePropertyGUI(property);
            }

            var root = _template.CloneTree();

            root.Add(new PropertyField(property.FindPropertyRelative(nameof(MotionTrack.clip))) { enabledSelf = false });
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(MotionTrack.mode))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(MotionTrack.settings))));

            var type = property.managedReferenceValue.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MotionTrack<>)) {
                root.Add(new PropertyField(property.FindPropertyRelative("useValueOverride")));
                root.Add(new PropertyField(property.FindPropertyRelative("value")));

            } else if (false /* type == typeof(ShakeMotionTrack) */) { // if type is ShakeMotionTrack
                // add after implementing ShakeMotionTrack
            }

            return root;
        }
    }
}