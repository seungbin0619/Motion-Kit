using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Move/Axis")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Move Axis Clip")]
    public sealed class MoveAxisClip : MotionClipWithValue<float> {        
#if UNITY_EDITOR
        public override string propertyKey => "position." + (axis == Axis.X ? "x" : axis == Axis.Y ? "y" : "z");
#endif

        [SerializeField]
        private bool isLocal;

        private enum Axis : byte { X, Y, Z };

        [SerializeField]
        private Axis axis;

        protected override Tween Create(Component target, TweenSettings<float> _settings) {
            if (target is Transform transform) {
                return (isLocal, axis) switch {
                    (true, Axis.X) => Tween.LocalPositionX(transform, _settings),
                    (true, Axis.Y) => Tween.LocalPositionY(transform, _settings),
                    (true, Axis.Z) => Tween.LocalPositionZ(transform, _settings),
                    (false, Axis.X) => Tween.PositionX(transform, _settings),
                    (false, Axis.Y) => Tween.PositionY(transform, _settings),
                    (false, Axis.Z) => Tween.PositionZ(transform, _settings),
                    _ => default
                };
            }

            return default;
        }

        public override void Ready(Component target, TweenValues<float> values) {
            if (values.startFromCurrent) return;

            if (target is Transform transform) {
                Vector3 position = isLocal ? transform.localPosition : transform.position;

                switch (axis) {
                    case Axis.X: position.x = values.startValue; break;
                    case Axis.Y: position.y = values.startValue; break;
                    case Axis.Z: position.z = values.startValue; break;
                }

                if (isLocal) {
                    transform.localPosition = position;
                } else {
                    transform.position = position;
                }
            }
        }
    }
}