using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackBaseElements : VisualElement {
        private readonly ObjectField _clipField;
        private readonly EnumField _modefield;
        private readonly TweenSettingsDrawer _settingsDrawer;

        public MotionTrackBaseElements() {
            _clipField = new("Clip") { enabledSelf = false };
            _modefield = new("Mode");
            _settingsDrawer = new();
            _settingsDrawer.style.marginTop = 8;

            _clipField.AddToClassList("sequence__field");
            _modefield.AddToClassList("sequence__field");

            Add(_clipField);
            Add(_modefield);
            Add(_settingsDrawer);
        }

        private void Unbind() {
            _clipField.Unbind();
            _modefield.Unbind();
            _settingsDrawer.Unbind();
        }

        public void BindProperty(SerializedProperty property) {
            if (property == null) {
                Unbind();
                return;
            }

            _clipField.BindProperty(property.FindPropertyRelative("clip"));
            _modefield.BindProperty(property.FindPropertyRelative("mode"));
            _settingsDrawer.BindProperty(property.FindPropertyRelative("settings"));
        }

        public void BindProperties(SerializedProperty[] properties, Action onChange = null) {
            _clipField.BindObjectProperties(properties, "clip", onChange);
            _modefield.BindEnumProperties(properties, "mode", typeof(TrackMode), 0, onChange);

            _settingsDrawer.BindProperties(properties, onChange);
        }
    }
}