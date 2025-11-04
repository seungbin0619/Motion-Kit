using System;
using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class TweenSettingsDrawer : VisualElement {
        private readonly FloatField _durationField;
        private readonly EnumField _easeField;
        private readonly PropertyField _customEaseField;
        private readonly IntegerField _cyclesField;
        private readonly FloatField _startDelayField;
        private readonly FloatField _endDelayField;
        private readonly Toggle _useUnscaledTimeField;
        private readonly EnumField _updateTypeField;

        public TweenSettingsDrawer() {
            var root = new VisualElement();
            root.AddToClassList("tween-settings-drawer");

            root.Add(_durationField = new("Duration"));
            root.Add(_startDelayField = new("Start Delay"));
            root.Add(_endDelayField = new("End Delay"));
            root.Add(_cyclesField = new("Cycles"));
            
            root.Add(_easeField = new("Ease"));
            root.Add(_customEaseField = new());
            
            root.Add(_useUnscaledTimeField = new("Use Unscaled Time"));
            root.Add(_updateTypeField = new("Update Type"));

            _durationField.AddToClassList("sequence__field");
            _startDelayField.AddToClassList("sequence__field");
            _endDelayField.AddToClassList("sequence__field");
            _cyclesField.AddToClassList("sequence__field");
            _easeField.AddToClassList("sequence__field");
            // _customEaseField.AddToClassList("sequence__field");
            _useUnscaledTimeField.AddToClassList("sequence__field");
            _updateTypeField.AddToClassList("sequence__field");

            _easeField.RegisterCallback<ChangeEvent<Enum>>(OnEaseChanged);    
            _durationField.RegisterCallback<ChangeEvent<float>>(ClampNegativeFloatValues);
            _startDelayField.RegisterCallback<ChangeEvent<float>>(ClampNegativeFloatValues);
            _endDelayField.RegisterCallback<ChangeEvent<float>>(ClampNegativeFloatValues);

            Add(root);
        }

        public void Unbind() {
            _durationField.Unbind();
            _easeField.Unbind();
            _customEaseField.Unbind();
            _cyclesField.Unbind();
            _startDelayField.Unbind();
            _endDelayField.Unbind();
            _useUnscaledTimeField.Unbind();
            _updateTypeField.Unbind();
        }

        public void BindProperty(SerializedProperty property) {
            if (property == null) {
                Unbind();
                return;
            }

            _durationField.BindProperty(property.FindPropertyRelative("duration"));
            _easeField.BindProperty(property.FindPropertyRelative("ease"));
            _customEaseField.SetEnabled(true);
            _customEaseField.BindProperty(property.FindPropertyRelative("customEase"));
            _cyclesField.BindProperty(property.FindPropertyRelative("cycles"));
            _startDelayField.BindProperty(property.FindPropertyRelative("startDelay"));
            _endDelayField.BindProperty(property.FindPropertyRelative("endDelay"));
            _useUnscaledTimeField.BindProperty(property.FindPropertyRelative("useUnscaledTime"));
            _updateTypeField.BindProperty(property.FindPropertyRelative("_updateType"));
        }

        public void BindProperties(SerializedProperty[] properties, Action onChange = null) {
            _durationField.BindFloatProperties(properties, "settings.duration", onChange);
            _easeField.BindEnumProperties(properties, "settings.ease", typeof(Ease), -1, onChange);
            _customEaseField.SetEnabled(false);
            _cyclesField.BindIntegerProperties(properties, "settings.cycles", onChange);
            _startDelayField.BindFloatProperties(properties, "settings.startDelay", onChange);
            _endDelayField.BindFloatProperties(properties, "settings.endDelay", onChange);
            _useUnscaledTimeField.BindBoolProperties(properties, "settings.useUnscaledTime", onChange);
            _updateTypeField.BindEnumProperties(properties, "settings._updateType", Type.GetType("PrimeTween._UpdateType, PrimeTween.Runtime"), 0, onChange);
        }

        private void OnEaseChanged(ChangeEvent<Enum> evt) {
            var easeEnumValue = (Ease)evt.newValue;

            _customEaseField.style.display = easeEnumValue == Ease.Custom 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
        }

        private void ClampNegativeFloatValues(ChangeEvent<float> evt) {
            var field = evt.target as FloatField;

            if (evt.newValue < 0f) {
                field.value = 0f;
                evt.StopPropagation();
            }
        }
    }
}