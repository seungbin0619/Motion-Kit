using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Move/Axis")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Anchored Move Axis Clip")]
    public sealed class AnchoredMoveAxisClip : MotionClipWithValue<float> {
#if UNITY_EDITOR
        public override string propertyKey => "anchoredPosition." + (axis == Axis.X ? "x" : "y");
#endif

        private enum Axis : byte { X, Y };

        [SerializeField]
        private Axis axis;

        protected override Tween Create(Component target, TweenSettings<float> _settings) {
            if (target is RectTransform rectTransform) {
                return axis switch {
                    Axis.X => Tween.UIAnchoredPositionX(rectTransform, _settings),
                    Axis.Y => Tween.UIAnchoredPositionY(rectTransform, _settings),
                    _ => default
                };
            }

            return default;
        }

        public override void Ready(Component target, TweenValues<float> values) {
            if (values.startFromCurrent) return;

            if (target is RectTransform rectTransform) {
                Vector2 position = rectTransform.anchoredPosition;

                switch (axis) {
                    case Axis.X: position.x = values.startValue; break;
                    case Axis.Y: position.y = values.startValue; break;
                }

                rectTransform.anchoredPosition = position;
            }
        }
    }
}