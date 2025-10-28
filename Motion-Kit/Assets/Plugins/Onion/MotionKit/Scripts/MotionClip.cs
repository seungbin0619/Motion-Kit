using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClip : ScriptableObject {
        protected abstract Tween CreateTween(Component target, TweenSettings settings);

#if UNITY_EDITOR
        public bool IsValidFor(Component target) {
            return CreateTween(target, default).isAlive;
        }
#endif
    }
}