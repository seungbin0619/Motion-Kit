using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Move")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Anchored Move Clip")]
    public sealed class AnchoredMoveClip : MotionClipWithValue<Vector2> {
#if UNITY_EDITOR
        public override string propertyKey => "anchoredPosition";
#endif

        protected override Tween Create(Component target, TweenSettings<Vector2> _settings) {
            if (target is not RectTransform rectTransform) return default;

            return Tween.UIAnchoredPosition(rectTransform, _settings);
        }
    }
}