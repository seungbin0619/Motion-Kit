using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomPropertyDrawer(typeof(MotionSignal))]
    public class MotionSignalDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var root = new VisualElement();

            var timeProp = property.FindPropertyRelative("time");
            var eventProp = property.FindPropertyRelative("onSignal");

            var timeField = new PropertyField(timeProp, "Time");
            var eventField = new PropertyField(eventProp, "OnSignal");
            eventField.AddToClassList("motion-signal-event-field");

            root.Add(timeField);
            root.Add(eventField);

            return root;
        }
    }
}