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

            return (isLocal, useShortestPath) switch {
                (true, true) => Tween.Rotation(transform, _settings),
                (true, false) => Tween.EulerAngles(transform, _settings),
                (false, true) => Tween.Rotation(transform, _settings),
                (false, false) => Tween.EulerAngles(transform, _settings),
            };
        }
    }
}