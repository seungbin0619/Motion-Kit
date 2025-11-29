using System.Collections.Generic;
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
        public abstract string propertyKey { get; }

        // public bool __IsConflic(MotionClip other) {
        //     if (__propertyKey == other.__propertyKey) return true;
        //     if (__propertyKey.StartsWith(other.__propertyKey + ".")) return true;
        //     if (other.__propertyKey.StartsWith(__propertyKey + ".")) return true;

        //     return false;
        // }

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