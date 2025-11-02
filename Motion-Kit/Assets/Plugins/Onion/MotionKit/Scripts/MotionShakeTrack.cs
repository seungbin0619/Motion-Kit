using System;
using PrimeTween;

namespace Onion.MotionKit {
    [Serializable]
    public class MotionShakeTrack : MotionTrack {
        public bool useValueOverride;
        public ShakeValues value;

        public override Tween Create() {
            if (clip is not MotionShakeClip shakeClip) {
                return base.Create();
            }

            if (useValueOverride) 
                return shakeClip.Create(target, settings, value);
            else return shakeClip.Create(target, settings);
        }
    }
}