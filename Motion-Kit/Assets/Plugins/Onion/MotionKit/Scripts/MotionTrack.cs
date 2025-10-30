using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionTrack {
        public int bindingIndex;
        public Component target;
        public MotionClip clip;
        public TweenSettings settings;

        public virtual Tween Create() {
            return clip.Create(target, settings);
        }
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        public bool useValueOverride;

        [ShowIf(nameof(useValueOverride))]
        public TweenValues<T> value;

        public override Tween Create() {
            if (clip is not MotionClipWithValue<T> valuedClip) return base.Create();

            if (useValueOverride) 
                return valuedClip.Create(target, settings, value);
            else return valuedClip.Create(target, settings);
        }
    }
}
