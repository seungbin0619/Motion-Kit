using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClipWithValue<T> : MotionClip where T : struct {
        [SerializeField]
        protected TweenValues<T> _values;

        public sealed override Tween Create(Component target, TweenSettings settings) {
            return Create(target, _values.ToSettings(settings));
        }

        public Tween Create(Component target, TweenSettings settings, TweenValues<T> values) {
            return Create(target, values.ToSettings(settings));
        }

        protected abstract Tween Create(Component target, TweenSettings<T> settings);
    }
}