using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006
    
    public class MotionTrackView : VisualElement {
        private static readonly IDictionary<MotionClipCategory, Color> _colorMap = new Dictionary<MotionClipCategory, Color> {
            { MotionClipCategory.None, new(0.6f, 0.6f, 0.6f) },
            { MotionClipCategory.Appear, new(0.4f, 0.8f, 0.4f) },
            { MotionClipCategory.Hide, new(0.8f, 0.4f, 0.4f) },
            { MotionClipCategory.Move, new(0.4f, 0.4f, 0.8f) },
            { MotionClipCategory.Rotate, new(0.8f, 0.8f, 0.4f) },
            { MotionClipCategory.Scale, new(0.4f, 0.8f, 0.8f) },
            { MotionClipCategory.Custom, new(0.8f, 0.4f, 0.8f) },
        };

        private SerializedProperty _trackProperty;
        public SerializedProperty trackProperty => _trackProperty;

        private readonly VisualElement _container;
        private readonly MotionSequenceView _parent;

        private VisualElement _trackTargetContainer;
        private VisualElement _trackTag;
        private PropertyField _trackTargetField;

        private VisualElement _trackTimelineContainer;

        public MotionTrackView(VisualTreeAsset template, MotionSequenceView parent = null) {
            if (template == null) return;
            Add(template.CloneTree()); 
            
            _container = this.Q<VisualElement>("track-container");
            _container.Add(_trackTargetContainer = CreateTrackTargetContainer());
            _container.Add(_trackTimelineContainer = CreateTrackTimelineContainer());

            _parent = parent;
            Repaint();
        }

        private VisualElement CreateTrackTargetContainer() {
            var container = new VisualElement();
            container.AddToClassList("track-target-container");

            _trackTag = new VisualElement();
            _trackTag.AddToClassList("track-tag");

            _trackTargetField = new PropertyField(null, label: "");
            _trackTargetField.AddToClassList("track-target-field");
            // _trackTargetField.dataSourceType = 

            container.Add(_trackTag);
            container.Add(_trackTargetField);

            return container;
        }

        private VisualElement CreateTrackTimelineContainer() {
            var container = new VisualElement();
            container.AddToClassList("track-timeline-container");

            return container; 
        }

        public void SetTrack(SerializedProperty trackProperty) {
            if (_trackProperty == trackProperty) return;

            _trackTargetField.Unbind();
            _trackProperty = trackProperty;
            
            if (_trackProperty == null) return;
            var trackTargetProp = _trackProperty.FindPropertyRelative("target");
            if (trackTargetProp != null) {
                _trackTargetField.BindProperty(trackTargetProp);
            }
            
            var color = _colorMap[MotionClipCategory.None];
            var clipProp = _trackProperty.FindPropertyRelative("clip");
            if (clipProp != null && clipProp.objectReferenceValue is MotionClip clip) {
                _colorMap.TryGetValue(clip.category, out color);
            }

            _trackTag.style.backgroundColor = color;
        }
        
        public void Repaint() {
            _trackTargetContainer.style.width = _parent.leftWidth;
        }
    }
}