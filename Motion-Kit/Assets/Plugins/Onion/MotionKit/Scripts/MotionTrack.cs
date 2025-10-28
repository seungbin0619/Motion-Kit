using System;
using PrimeTween;
using UnityEngine;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionTrack {
        [SerializeField]
        private int _bindingIndex;
    }

    [Serializable]
    public class MotionTrack<T> : MotionTrack where T : struct {
        
    }
}
