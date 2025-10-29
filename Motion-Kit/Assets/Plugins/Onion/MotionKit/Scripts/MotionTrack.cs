using System;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionTrack {
        public int bindingIndex;
        
        public Component target;
        public TweenSettings settings;
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        public bool useValueOverride;

        [ShowIf(nameof(useValueOverride))]
        public TweenValues<T> value;
    }
}
