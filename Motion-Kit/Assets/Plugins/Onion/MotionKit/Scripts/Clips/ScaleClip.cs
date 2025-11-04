using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Scale")]
    [CreateAssetMenu(fileName = "ScaleClip", menuName = "Onion/Motion Kit/Clips/Scale Clip")]
    public class ScaleClip : MotionClipWithValue<Vector3> {
        protected override Tween Create(Component target, TweenSettings<Vector3> _settings) {
            return target switch {
                Transform transform => Tween.Scale(transform, _settings),
                _ => default
            };
        }
    }
}