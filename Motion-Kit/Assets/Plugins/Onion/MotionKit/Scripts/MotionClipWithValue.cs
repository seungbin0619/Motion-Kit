using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClipWithValue<T> : MotionClip where T : struct {
        [SerializeField]
        protected TweenValues<T> _values;

        protected sealed override Tween CreateTween(Component target, TweenSettings settings) {
            return CreateTween(target, _values.ToSettings(settings));
        }

        protected Tween CreateTween(Component target, TweenSettings settings, TweenValues<T> values) {
            return CreateTween(target, values.ToSettings(settings));
        }

        protected abstract Tween CreateTween(Component target, TweenSettings<T> settings);
    }
}