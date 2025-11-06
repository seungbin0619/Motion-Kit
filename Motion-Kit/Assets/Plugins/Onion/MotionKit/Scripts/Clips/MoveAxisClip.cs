using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Move/Axis")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Move Axis Clip")]
    public sealed class MoveAxisClip : MotionClipWithValue<float> {
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
    }
}