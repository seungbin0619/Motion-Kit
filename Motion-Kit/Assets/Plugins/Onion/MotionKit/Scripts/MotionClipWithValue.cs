using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClipWithValue<T> : MotionClip where T : struct {
        public TweenValues<T> value;

        public sealed override Tween Create(Component target, TweenSettings settings) {
            return Create(target, value.ToSettings(settings));
        }

        public Tween Create(Component target, TweenSettings settings, TweenValues<T> values) {
            return Create(target, values.ToSettings(settings));
        }

        protected abstract Tween Create(Component target, TweenSettings<T> settings);

        public override void Ready(Component target) => Ready(target, value);
        public abstract void Ready(Component target, TweenValues<T> values);
    }
}