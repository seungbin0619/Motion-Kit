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
            SetEnabled(false);
            var tween = Create(target, default);
            var valid = tween.isAlive;
            tween.Stop();
            SetEnabled(true);

            return valid;
        }

        // ignore PrimeTween warnings in editor validation
        private static void SetEnabled(bool enabled) {
            PrimeTweenConfig.warnZeroDuration = enabled;
            PrimeTweenConfig.warnEndValueEqualsCurrent = enabled;
        }
#endif
    }
}