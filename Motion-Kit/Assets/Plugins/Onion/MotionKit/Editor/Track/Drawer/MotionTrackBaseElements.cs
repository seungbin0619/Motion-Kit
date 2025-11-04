using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackBaseElements : VisualElement {
        private readonly PropertyField _clipField;
        private readonly PropertyField _modefield;
        private readonly TweenSettingsDrawer _settingsDrawer;

        public MotionTrackBaseElements() {
            _clipField = new() { enabledSelf = false };
            _modefield = new();
            _settingsDrawer = new();

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

            _clipField.BindProperty(property.FindPropertyRelative(nameof(MotionTrack.clip)));
            _modefield.BindProperty(property.FindPropertyRelative(nameof(MotionTrack.mode)));
            _settingsDrawer.BindProperty(property.FindPropertyRelative(nameof(MotionTrack.settings)));
        }
    }
}