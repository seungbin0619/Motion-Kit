using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public class FadeClip : MotionClip<float> {
        protected override Tween CreateTween(Component target, TweenSettings<float> settings) {
            return target switch {
                CanvasGroup canvasGroup => Tween.Alpha(canvasGroup, settings),
                SpriteRenderer spriteRenderer => Tween.Alpha(spriteRenderer, settings),
                UnityEngine.UI.Graphic uiGraphic => Tween.Alpha(uiGraphic, settings),
                UnityEngine.UI.Shadow uiShadow => Tween.Alpha(uiShadow, settings),
                
                _ => default
            };
        }

        public override bool IsValidFor(Component target) {
            return target 
                is CanvasGroup 
                or SpriteRenderer 
                or UnityEngine.UI.Graphic 
                or UnityEngine.UI.Shadow;
        }
    }
}