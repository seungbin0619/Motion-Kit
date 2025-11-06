using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackBaseElements : VisualElement {
        private readonly ObjectField _clipField;
        private readonly EnumField _modefield;
        private readonly Toggle _independentToggle;
        private readonly FloatField _delayField;
        private readonly TweenSettingsDrawer _settingsDrawer;

        public MotionTrackBaseElements() {
            _clipField = new("Clip") { enabledSelf = false };
            _modefield = new("Mode");
            _independentToggle = new("Run Independently");
            _delayField = new("Delay");

            _settingsDrawer = new();
            _settingsDrawer.style.marginTop = 8;

            _clipField.AddToClassList("sequence__field");
            _modefield.AddToClassList("sequence__field");
            _independentToggle.AddToClassList("sequence__field");
            _delayField.AddToClassList("sequence__field");

            Add(_clipField);
            Add(_modefield);
            Add(_independentToggle);
            Add(_delayField);

            Add(_settingsDrawer);

            _delayField.RegisterValueChangedCallback(ClampNegativeFloatValues);
        }

        private void Unbind() {
            _clipField.Unbind();
            _modefield.Unbind();
            _independentToggle.Unbind();
            _delayField.Unbind();
            _settingsDrawer.Unbind();
        }

        public void BindProperty(SerializedProperty property) {
            if (property == null) {
                Unbind();
                return;
            }

            _clipField.BindProperty(property.FindPropertyRelative("clip"));
            _modefield.BindProperty(property.FindPropertyRelative("mode"));
            _independentToggle.BindProperty(property.FindPropertyRelative("runIndependently"));
            _delayField.BindProperty(property.FindPropertyRelative("delay"));
            _settingsDrawer.BindProperty(property.FindPropertyRelative("settings"));
        }

        public void BindProperties(SerializedProperty[] properties, Action onChange = null) {
            _clipField.BindObjectProperties(properties, "clip", onChange);
            _modefield.BindEnumProperties(properties, "mode", typeof(TrackMode), 0, onChange);
            _independentToggle.BindBoolProperties(properties, "runIndependently", onChange);
            _delayField.BindFloatProperties(properties, "delay", onChange);

            _settingsDrawer.BindProperties(properties, onChange);
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