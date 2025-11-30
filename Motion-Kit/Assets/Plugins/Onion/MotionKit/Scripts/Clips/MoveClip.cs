using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Move")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Move Clip")]
    public sealed class MoveClip : MotionClipWithValue<Vector3> {
#if UNITY_EDITOR
        public override string propertyKey => "position";
#endif

        [SerializeField]
        private bool _isLocal;

        protected override Tween Create(Component target, TweenSettings<Vector3> _settings) {
            if (target is not Transform transform) return default;

            return _isLocal
                ? Tween.LocalPosition(transform, _settings)
                : Tween.Position(transform, _settings);
        }

        public override void Ready(Component target, TweenValues<Vector3> values) {
            if (values.startFromCurrent) return;

            if (target is Transform transform) {
                if (_isLocal) {
                    transform.localPosition = values.startValue;
                } else {
                    transform.position = values.startValue;
                }
            }
        }
    }
}