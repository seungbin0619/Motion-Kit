using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackView : VisualElement {
        private SerializedProperty _trackProperty;
        private VisualElement _container;

        public MotionTrackView(VisualTreeAsset template) {
            if (template == null) return;

            Add(template.CloneTree()); 
            _container = this.Q<VisualElement>("track-container");
            
            _container.style.backgroundColor = new StyleColor(
                new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 0.1f));
        }

        public void SetTrack(SerializedProperty trackProperty) {
            _trackProperty = trackProperty;
        }
    }
}