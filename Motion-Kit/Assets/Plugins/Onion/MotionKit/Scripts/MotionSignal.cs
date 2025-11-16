using System;
using UnityEngine;
using UnityEngine.Events;

namespace Onion.MotionKit {
    [Serializable]
    public sealed class MotionSignal {
        public float time;
        public UnityEvent onSignal = new();
    }
}