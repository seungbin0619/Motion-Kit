using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    public abstract class MotionClip : ScriptableObject {
        public abstract Tween Create(Component target, TweenSettings settings);

#if UNITY_EDITOR
        public bool IsValidFor(Component target) {
            return Create(target, default).isAlive;
        }
#endif
    }
}