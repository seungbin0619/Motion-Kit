using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Rotate")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Rotate Clip")]
    public sealed class RotateClip : MotionClipWithValue<Vector3> {
        [SerializeField]
        private bool isLocal = true;

        [SerializeField]
        private bool useShortestPath = true;

        protected override Tween Create(Component target, TweenSettings<Vector3> _settings) {
            if (target is not Transform transform) return default;

            if (useShortestPath) {
                return isLocal
                    ? Tween.Rotation(transform, _settings)
                    : Tween.Rotation(transform, _settings);
            } else {
                return isLocal
                    ? Tween.EulerAngles(transform, _settings)
                    : Tween.EulerAngles(transform, _settings);
            }
        }
    }
}