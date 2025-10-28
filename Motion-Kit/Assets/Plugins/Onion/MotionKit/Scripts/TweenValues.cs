using System;

namespace Onion.MotionKit {
    [Serializable]
    public struct TweenValues<T> where T : struct {
        public bool startFromCurrent;
        public T startValue;
        public T endValue;

        public TweenValues(T startValue, T endValue) {
            startFromCurrent = false;

            this.startValue = startValue;
            this.endValue = endValue;
        }

        public TweenValues(T endValue) {
            startFromCurrent = true;

            startValue = default;
            this.endValue = endValue;
        }
    }
}