using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Scale")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Scale Clip")]
    public sealed class ScaleClip : MotionClipWithValue<Vector3> {
#if UNITY_EDITOR
        public override string propertyKey => "scale";
#endif
        protected override Tween Create(Component target, TweenSettings<Vector3> _settings) {
            return target switch {
                Transform transform => Tween.Scale(transform, _settings),
                _ => default
            };
        }
    }
}