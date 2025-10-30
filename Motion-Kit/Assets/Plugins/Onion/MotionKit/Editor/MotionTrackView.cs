using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006
    
    public class MotionTrackView : VisualElement {
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
            
            _trackTag.style.backgroundColor = Color.red;
        }
        
        public void Repaint() {
            _trackTargetContainer.style.width = _parent.leftWidth;
        }
    }
}