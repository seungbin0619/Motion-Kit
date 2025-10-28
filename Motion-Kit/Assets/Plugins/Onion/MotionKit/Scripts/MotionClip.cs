using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClip<T> : ScriptableObject where T : struct {
        [SerializeField]
        private TweenValues<T> _values;

        protected abstract Tween CreateTween(Component target, TweenSettings<T> settings);

#if UNITY_EDITOR
        public bool IsValidFor(Component target) {
            return CreateTween(target, default).isAlive;
        }
#endif
    }
}