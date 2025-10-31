using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceTimeRuler : VisualElement {
        private readonly MotionSequenceView _parent;

        public MotionSequenceTimeRuler(MotionSequenceView parent = null) {
            _parent = parent;

            AddToClassList("time-ruler-container");
        }

        public void Repaint() {
            Clear();
            if (_parent == null) return;

            style.marginLeft = _parent.leftWidth + 2;
            var totalWidth = _parent.contentContainer.layout.width - _parent.leftWidth - 2;

            float second = Mathf.Ceil(_parent.startTime * 2.0f) * 0.5f - 1;
            float position = _parent.minMarginLeft + 
                second * _parent.pixelsPerSecond - 
                _parent.startTime * _parent.pixelsPerSecond;

            while (position < totalWidth) {
                if (second % 1.0f == 0) {
                    var tick = new VisualElement();
                    tick.AddToClassList("time-ruler-tick");
                    tick.style.left = position;
                    Add(tick);

                    var label = new Label($"{second}");
                    label.AddToClassList("time-ruler-label");
                    label.style.left = position;
                    Add(label);
                } else {
                    var halfTick = new VisualElement();
                    halfTick.AddToClassList("time-ruler-tick");
                    halfTick.AddToClassList("half-tick");
                    halfTick.style.left = position;
                    Add(halfTick);
                }

                second += 0.5f;
                position += _parent.pixelsPerSecond * 0.5f;
            }
        }
    }
}