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

        public TweenSettingsDrawer(SerializedProperty property) {
            var root = new VisualElement();
            root.AddToClassList("tween-settings-drawer");

            _durationField = new(property.FindPropertyRelative(nameof(TweenSettings.duration)));
            _easeField = new(property.FindPropertyRelative(nameof(TweenSettings.ease)));
            _customEaseField = new(property.FindPropertyRelative(nameof(TweenSettings.customEase)));
            _cyclesField = new(property.FindPropertyRelative(nameof(TweenSettings.cycles)));
            _startDelayField = new(property.FindPropertyRelative(nameof(TweenSettings.startDelay)));
            _endDelayField = new(property.FindPropertyRelative(nameof(TweenSettings.endDelay)));
            _useUnscaledTimeField = new(property.FindPropertyRelative(nameof(TweenSettings.useUnscaledTime)));
            _updateTypeField = new(property.FindPropertyRelative("_updateType"));

            root.Add(_durationField);
            root.Add(_startDelayField);
            root.Add(_endDelayField);
            root.Add(_cyclesField);
            
            root.Add(_easeField);
            root.Add(_customEaseField);
            
            root.Add(_useUnscaledTimeField);
            root.Add(_updateTypeField);

            _easeField.RegisterCallback<SerializedPropertyChangeEvent>(OnEaseChanged);    
            _durationField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);
            _startDelayField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);
            _endDelayField.RegisterCallback<SerializedPropertyChangeEvent>(ClampNegativeFloatValues);

            Add(root);
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