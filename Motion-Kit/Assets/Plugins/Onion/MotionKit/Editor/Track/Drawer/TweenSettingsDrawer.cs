using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class TweenSettingsDrawer : VisualElement {
        private readonly PropertyField _durationField;
        private readonly PropertyField _easeField;
        private readonly PropertyField _customEaseField;
        private readonly PropertyField _cyclesField;
        private readonly PropertyField _startDelayField;
        private readonly PropertyField _endDelayField;
        private readonly PropertyField _useUnscaledTimeField;
        private readonly PropertyField _updateTypeField;

        public TweenSettingsDrawer() {
            var root = new VisualElement();
            root.AddToClassList("tween-settings-drawer");

            root.Add(_durationField = new());
            root.Add(_startDelayField = new());
            root.Add(_endDelayField = new());
            root.Add(_cyclesField = new());
            
            root.Add(_easeField = new());
            root.Add(_customEaseField = new());
            
            root.Add(_useUnscaledTimeField = new());
            root.Add(_updateTypeField = new());

            _easeField.RegisterCallback<SerializedPropertyChangeEvent>(OnEaseChanged);    
            _durationField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);
            _startDelayField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);
            _endDelayField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);

            Add(root);
        }

        public void BindProperty(SerializedProperty property) {
            _durationField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.duration)));
            _easeField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.ease)));
            _customEaseField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.customEase)));
            _cyclesField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.cycles)));
            _startDelayField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.startDelay)));
            _endDelayField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.endDelay)));
            _useUnscaledTimeField.BindProperty(property.FindPropertyRelative(nameof(TweenSettings.useUnscaledTime)));
            _updateTypeField.BindProperty(property.FindPropertyRelative("_updateType"));
        }

        private void OnEaseChanged(SerializedPropertyChangeEvent evt) {
            var property = evt.changedProperty;
            var easeEnumValue = (Ease)(property.enumValueIndex - 1);

            _customEaseField.style.display = easeEnumValue == Ease.Custom 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        private void ClampNegativeFloatValues(SerializedPropertyChangeEvent evt) {
            var property = evt.changedProperty;

            if (property.floatValue < 0f) {
                property.floatValue = 0f;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}