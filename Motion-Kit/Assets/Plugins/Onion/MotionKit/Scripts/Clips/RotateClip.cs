using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Rotate")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Rotate Clip")]
    public sealed class RotateClip : MotionClipWithValue<Vector3> {
        [SerializeField]
        private bool isLocal = true;
        
        protected override Tween Create(Component target, TweenSettings<Vector3> _settings) {
            if (isLocal) {
                return target switch {
                    Transform transform => Tween.LocalRotation(transform, _settings),
                    _ => default
                };
            } else {
                return target switch {
                    Transform transform => Tween.Rotation(transform, _settings),
                    _ => default
                };
            }
        }
    }
}