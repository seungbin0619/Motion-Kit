using System;
using PrimeTween;
using Sirenix.OdinInspector;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionTrack {
        public int bindingIndex;

        public TweenSettings settings;
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        public bool useValueOverride;

        [ShowIf(nameof(useValueOverride))]
        public TweenValues<T> value;
    }
}
