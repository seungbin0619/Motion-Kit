using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    #pragma warning disable IDE1006

    [Serializable]
    public class MotionTrack {
        public Component target;
        public MotionClip clip;
        public TrackMode mode;
        public bool runIndependently;
        public float delay;
        
        public TweenSettings settings;
        public bool isValid => target != null && clip != null;
        
        public float totalDuration {
            get {
                var cycles = settings.cycles;
                if (cycles < 0) return float.PositiveInfinity;

                cycles = Mathf.Max(1, cycles);
                return delay + (settings.startDelay + settings.duration + settings.endDelay) * cycles;
            }
        }

        public MotionTrack() {
            settings.duration = 1f;
        }

        public virtual Tween Create() {
            return clip.Create(target, settings);
        }
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        public bool useValueOverride;
        public TweenValues<T> value;

        public MotionTrack(MotionClipWithValue<T> clip) {
            value = clip.value;
        }

        public override Tween Create() {
            if (clip is not MotionClipWithValue<T> valuedClip) {
                return base.Create();
            }

            if (useValueOverride) 
                return valuedClip.Create(target, settings, value);
            else return valuedClip.Create(target, settings);
        }
    }
}
