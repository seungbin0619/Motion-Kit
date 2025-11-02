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

            return root;
        }
    }
}