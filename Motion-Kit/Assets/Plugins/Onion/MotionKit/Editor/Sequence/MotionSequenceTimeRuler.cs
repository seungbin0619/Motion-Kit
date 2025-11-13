using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceTimeRuler : VisualElement {
        private readonly MotionSequenceView _parent;
        private readonly VisualElement _mainContainer;
        private readonly VisualElement _signalContainer;

        public MotionSequenceTimeRuler(MotionSequenceView parent = null) {
            _parent = parent;
            
            Add(_mainContainer = new VisualElement());
            Add(_signalContainer = new VisualElement());

            _mainContainer.AddToClassList("time-ruler-main-container");
            _signalContainer.AddToClassList("time-ruler-signal-container");

            AddToClassList("time-ruler-container");
        }

        public void Repaint(SerializedProperty property = null) {
            _mainContainer.Clear();
            if (_parent == null) return;

            style.marginLeft = _parent.leftWidth + 2;

            float second = Mathf.Ceil(_parent.startTime * 2.0f) * 0.5f - 1;
            float position = _parent.minMarginLeft + (second - _parent.startTime) * _parent.pixelsPerSecond;

            while (position < _parent.totalWidth) {
                if (second % 1.0f == 0) {
                    var tick = new VisualElement();
                    tick.AddToClassList("time-ruler-tick");
                    tick.style.left = position;
                    _mainContainer.Add(tick);

                    var label = new Label($"{second}");
                    label.AddToClassList("time-ruler-label");
                    label.style.left = position;
                    _mainContainer.Add(label);
                } else {
                    var halfTick = new VisualElement();
                    halfTick.AddToClassList("time-ruler-tick");
                    halfTick.AddToClassList("half-tick");
                    halfTick.style.left = position;
                    _mainContainer.Add(halfTick);
                }

                second += 0.5f;
                position += _parent.pixelsPerSecond * 0.5f;
            }

            float from = _parent.startTime - _parent.minMarginLeft / _parent.pixelsPerSecond;
            float to = from + _parent.totalWidth / _parent.pixelsPerSecond;

            if (property == null) return;

            _signalContainer.Clear();

            for (int i = 0; i < property.arraySize; i++) {
                var signalProperty = property.GetArrayElementAtIndex(i);
                float signalTime = signalProperty.FindPropertyRelative("time").floatValue;

                if (signalTime < from || signalTime > to) continue;

                float signalPosition = _parent.minMarginLeft + (signalTime - _parent.startTime) * _parent.pixelsPerSecond;

                var signalMarker = new VisualElement() { focusable = true };
                signalMarker.AddToClassList("motion-signal");

                signalMarker.style.left = signalPosition;
                _signalContainer.Add(signalMarker);
            }
        }
    }
}