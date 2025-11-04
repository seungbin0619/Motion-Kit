using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class TweenSettingsDrawer : VisualElement {
        public TweenSettingsDrawer(SerializedProperty property) {
            var root = new VisualElement();

            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.duration))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.ease))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.customEase))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.cycles))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.startDelay))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.endDelay))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.useUnscaledTime))));
            root.Add(new PropertyField(property.FindPropertyRelative(nameof(TweenSettings.updateType))));

            Add(root);
        }
    }
}