using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Shake")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Shake Clip")]
    public sealed class MotionShakeClip : MotionClip {
#if UNITY_EDITOR
        public override string propertyKey => string.Empty;
#endif
        private enum ShakeMode {
            None,
            Position,
            Rotation,
            Scale
        }

        [SerializeField]
        private ShakeMode mode;

        [SerializeField]
        private bool isPunch;

        public ShakeValues value;

        public sealed override Tween Create(Component target, TweenSettings settings) {
            return Create(target, value.ToSettings(settings));
        }

        public Tween Create(Component target, TweenSettings settings, ShakeValues values) {
            return Create(target, values.ToSettings(settings));
        }

        private Tween Create(Component target, ShakeSettings settings) {
            if (target is not Transform transform) return default;
            if (isPunch) {
                return mode switch {
                    ShakeMode.Position => Tween.PunchLocalPosition(transform, settings),
                    ShakeMode.Rotation => Tween.PunchLocalRotation(transform, settings),
                    ShakeMode.Scale => Tween.PunchScale(transform, settings),
                    _ => default,
                };
            } else {
                return mode switch {
                    ShakeMode.Position => Tween.ShakeLocalPosition(transform, settings),
                    ShakeMode.Rotation => Tween.ShakeLocalRotation(transform, settings),
                    ShakeMode.Scale => Tween.ShakeScale(transform, settings),
                    _ => default,
                };
            }
        }
    }
}