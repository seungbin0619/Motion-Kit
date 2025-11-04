using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomPropertyDrawer(typeof(MotionTrack), useForChildren: true)]
    public class MotionTrackDrawer : PropertyDrawer {
        [SerializeField]
        private StyleSheet styleSheet;

        private SerializedProperty _property;

        private PropertyField _clipField;
        private PropertyField _modefield;
        private TweenSettingsDrawer _settingsDrawer;
        private PropertyField _useValueOverrideField;
        private PropertyField _valueField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            _property = property;

            var root = new VisualElement();
            root.AddToClassList("motion-track-drawer");
            root.styleSheets.Add(styleSheet);

            _clipField = new(property.FindPropertyRelative(nameof(MotionTrack.clip))) { enabledSelf = false };
            _modefield = new(property.FindPropertyRelative(nameof(MotionTrack.mode)));
            _settingsDrawer = new(property.FindPropertyRelative(nameof(MotionTrack.settings)));

            root.Add(_clipField);
            root.Add(_modefield);
            root.Add(_settingsDrawer);

            var type = property.managedReferenceValue.GetType();
            if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MotionTrack<>)) || type == typeof(MotionShakeTrack)) {
                _useValueOverrideField = new PropertyField(property.FindPropertyRelative("useValueOverride"));
                _valueField = new PropertyField(property.FindPropertyRelative("value"));

                root.Add(_useValueOverrideField);
                root.Add(_valueField);

                _useValueOverrideField.RegisterCallback<ChangeEvent<bool>>(OnValueOverrideChanged);
            } else {
                // for custom motion tracks
                // implement after needed
            }

            return root;
        }

        private void OnValueOverrideChanged(ChangeEvent<bool> evt) {
            var prop = evt.newValue
                ? _property.FindPropertyRelative("value")
                : new SerializedObject(_property.FindPropertyRelative("clip").objectReferenceValue).FindProperty("value");

            _valueField.SetEnabled(evt.newValue);
            if (prop == null) return;
            
            _valueField.Unbind();
            _valueField.BindProperty(prop);
        }
    }
}