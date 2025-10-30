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

            var second = 0f;
            var position = _parent.minMarginLeft;
            var totalWidth = _parent.contentContainer.layout.width - _parent.leftWidth - 2;

            while (position < totalWidth) {
                var tick = new VisualElement();
                tick.AddToClassList("time-ruler-tick");
                tick.style.left = position;
                Add(tick);

                var label = new Label($"{second}");
                label.AddToClassList("time-ruler-label");
                label.style.left = position;
                Add(label);

                second += 1f;
                position += _parent.pixelPerSecond;
            }
        }
    }
}