using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackView : VisualElement {
        private SerializedProperty _trackProperty;

        public MotionTrackView(VisualTreeAsset template) {
            if (template == null) return;

            Add(template.CloneTree()); 
        }

        public void SetTrack(SerializedProperty trackProperty) {
            _trackProperty = trackProperty;
        }
    }
}