using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomPropertyDrawer(typeof(ShakeValues))]
    public class ShakeValuesDrawer : PropertyDrawer {
        private PropertyField _strengthField;
        private PropertyField _frequencyField;
        private PropertyField _enableFalloffField;
        private PropertyField _falloffEaseField;
        private PropertyField _strengthOverTimeField;
        private PropertyField _asymmetryField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var root = new VisualElement();

            _strengthField = new(property.FindPropertyRelative(nameof(ShakeValues.strength)));
            _frequencyField = new(property.FindPropertyRelative(nameof(ShakeValues.frequency)));
            _enableFalloffField = new(property.FindPropertyRelative(nameof(ShakeValues.enableFalloff)));
            _falloffEaseField = new(property.FindPropertyRelative(nameof(ShakeValues.falloffEase)));
            _strengthOverTimeField = new(property.FindPropertyRelative(nameof(ShakeValues.strengthOverTime)));
            _asymmetryField = new(property.FindPropertyRelative(nameof(ShakeValues.asymmetry)));

            root.Add(_strengthField);
            root.Add(_frequencyField);
            root.Add(_enableFalloffField);
            root.Add(_falloffEaseField);
            root.Add(_strengthOverTimeField);
            root.Add(_asymmetryField);
            
            _enableFalloffField.RegisterValueChangeCallback(OnEnableFalloffChanged);
            _falloffEaseField.RegisterValueChangeCallback(OnFalloffEaseChanged);

            return root;
        }

        private void OnEnableFalloffChanged(SerializedPropertyChangeEvent evt) {
            var property = evt.changedProperty;
            
            _falloffEaseField.style.display = property.boolValue
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void OnFalloffEaseChanged(SerializedPropertyChangeEvent evt) {
            var property = evt.changedProperty;
            var ease = (Ease)(property.enumValueIndex - 1);

            _strengthOverTimeField.style.display = ease == Ease.Custom
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }
    }
}