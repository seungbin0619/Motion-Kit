using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomPropertyDrawer(typeof(MotionTrack), useForChildren: true)]
    public class MotionTrackDrawer : PropertyDrawer {
        [SerializeField]
        private StyleSheet styleSheet;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var root = new VisualElement();
            root.styleSheets.Add(styleSheet);

            root.Add(new PropertyField(property.FindPropertyRelative(nameof(MotionTrack.clip))) { enabledSelf = false });
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(MotionTrack.mode))));
            root.Add(new TweenSettingsDrawer(property.FindPropertyRelative(nameof(MotionTrack.settings))));

            var type = property.managedReferenceValue.GetType();
            if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MotionTrack<>)) || type == typeof(MotionShakeTrack)) {
                root.Add(new PropertyField(property.FindPropertyRelative("useValueOverride")));
                root.Add(new PropertyField(property.FindPropertyRelative("value")));
            } else {
                // for custom motion tracks
                // implement after needed
            }

            return root;
        }
    }
}