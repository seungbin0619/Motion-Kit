using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [MotionClipMenu("Fade")]
    [CreateAssetMenu(menuName = "Animation/Motion Kit/Fade Clip")]
    public sealed class FadeClip : MotionClipWithValue<float> {
#if UNITY_EDITOR
        public override string propertyKey => "alpha";
#endif

        protected override Tween Create(Component target, TweenSettings<float> _settings) {
            return target switch {
                CanvasGroup canvasGroup => Tween.Alpha(canvasGroup, _settings),
                SpriteRenderer spriteRenderer => Tween.Alpha(spriteRenderer, _settings),
                UnityEngine.UI.Graphic uiGraphic => Tween.Alpha(uiGraphic, _settings),
                UnityEngine.UI.Shadow uiShadow => Tween.Alpha(uiShadow, _settings),

                _ => default
            };
        }

        public override void Ready(Component target, TweenValues<float> values) {
            if (values.startFromCurrent) return;

            Color color;
            switch (target) {
                case CanvasGroup canvasGroup:
                    canvasGroup.alpha = values.startValue;
                    break;
                case SpriteRenderer spriteRenderer:
                    color = spriteRenderer.color;
                    color.a = values.startValue;
                    spriteRenderer.color = color;
                    break;
                case UnityEngine.UI.Graphic uiGraphic:
                    color = uiGraphic.color;
                    color.a = values.startValue;
                    uiGraphic.color = color;
                    break;
                case UnityEngine.UI.Shadow uiShadow:
                    color = uiShadow.effectColor;
                    color.a = values.startValue;
                    uiShadow.effectColor = color;
                    break;
            }
        }
    }
}