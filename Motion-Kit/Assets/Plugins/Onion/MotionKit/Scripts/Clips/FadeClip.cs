using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Fade")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Fade Clip")]
    public sealed class FadeClip : MotionClipWithValue<float> {
        protected override Tween Create(Component target, TweenSettings<float> _settings) {
            return target switch {
                CanvasGroup canvasGroup => Tween.Alpha(canvasGroup, _settings),
                SpriteRenderer spriteRenderer => Tween.Alpha(spriteRenderer, _settings),
                UnityEngine.UI.Graphic uiGraphic => Tween.Alpha(uiGraphic, _settings),
                UnityEngine.UI.Shadow uiShadow => Tween.Alpha(uiShadow, _settings),

                _ => default
            };
        }
    }
}