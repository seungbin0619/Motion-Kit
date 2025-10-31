using System;

namespace Onion.MotionKit {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MotionClipMenu : Attribute {
        public readonly string path;

        public MotionClipMenu(string path) {
            this.path = path;
        }
    }
}