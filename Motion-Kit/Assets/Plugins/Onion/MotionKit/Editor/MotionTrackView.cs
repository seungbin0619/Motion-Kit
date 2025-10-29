using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionTrackView : VisualElement {
        private SerializedProperty _trackProperty;
        private readonly VisualElement _container;
        private readonly MotionSequenceView _parent;
        private PropertyField _trackTargetField;
        private VisualElement _trackTimelineContainer;

        public MotionTrackView(VisualTreeAsset template, MotionSequenceView parent = null) {
            if (template == null) return;
            Add(template.CloneTree()); 
            
            _container = this.Q<VisualElement>("track-container");
            _container.Add(CreateTrackTargetContainer());
            _container.Add(_trackTimelineContainer = CreateTrackTimelineContainer());

            _parent = parent;
        }

        private VisualElement CreateTrackTargetContainer() {
            var container = new VisualElement();
            container.AddToClassList("track-target-container");

            _trackTargetField = new PropertyField(null, label: "");
            _trackTargetField.AddToClassList("track-target-field");
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
            _trackTargetField.BindProperty(_trackProperty.FindPropertyRelative("target"));
        }
    }
}