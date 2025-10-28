using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    internal struct TweenValues<T> where T : struct {
        public T startValue;
        public T endValue;
    }

    public abstract class MotionClip<T> : ScriptableObject where T : struct {
        [SerializeField]
        internal TweenValues<T> _values;

        protected abstract Tween CreateTween(Component target, TweenSettings<T> settings);
        
        public bool IsValidFor(Component target) {
            return CreateTween(target, default).isAlive;
        }
    }
}