using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    #pragma warning disable IDE1006
    
    public enum MotionClipCategory {
        None,
        Appear,
        Hide,
        Move,
        Rotate,
        Scale,
        Custom,
    }

    public abstract class MotionClip : ScriptableObject {
        [field: SerializeField]
        public MotionClipCategory category { get; private set; }

        public abstract Tween Create(Component target, TweenSettings settings);

#if UNITY_EDITOR
        public bool IsValidFor(Component target) {
            return Create(target, default).isAlive;
        }
#endif
    }
}