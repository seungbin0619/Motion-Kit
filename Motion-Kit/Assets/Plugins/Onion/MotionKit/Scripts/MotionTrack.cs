using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionTrack {
        public int bindingIndex;
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        
    }
}
