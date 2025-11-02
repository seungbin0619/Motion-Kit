using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    #pragma warning disable IDE0017 // ignore simplify object initialization
    
    [Serializable]
    public struct ShakeValues {
        public Vector3 strength;
        public float frequency;
        public bool enableFalloff;
        public Ease falloffEase;
        public AnimationCurve strengthOverTime;
        public float asymmetry;

        public readonly ShakeSettings ToSettings(TweenSettings settings) {
            var result = new ShakeSettings();

            result.strength = strength;
            result.duration = settings.duration;
            result.frequency = frequency;

            result.enableFalloff = enableFalloff;
            if (enableFalloff) {
                result.falloffEase = falloffEase;
                result.strengthOverTime = falloffEase == Ease.Custom ? strengthOverTime : null;
            }
            
            result.asymmetry = asymmetry;
            result.cycles = settings.cycles;
            result.startDelay = settings.startDelay;
            result.endDelay = settings.endDelay;
            result.useUnscaledTime = settings.useUnscaledTime;
            result.updateType = settings.updateType;

            return result;
        }
    }
}