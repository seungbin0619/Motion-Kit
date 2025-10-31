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
        private VisualElement _realTrackTimeline;
        private Label _realTrackLabel;
        private VisualElement _realTrackTag;

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
            _trackTargetField.RegisterValueChangeCallback(OnTargetChanged);

            container.Add(_trackTag);
            container.Add(_trackTargetField);

            return container;
        }

        private VisualElement CreateTrackTimelineContainer() {
            var container = new VisualElement();
            container.AddToClassList("track-timeline-container");

            _realTrackTimeline = new VisualElement();
            _realTrackTimeline.AddToClassList("track-timeline-content");
            // _realTrackTimeline.RegisterCallback<PointerDownEvent>(OnPointerDown);
            // _realTrackTimeline.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            // _realTrackTimeline.RegisterCallback<PointerUpEvent>(OnPointerUp);

            _realTrackLabel = new Label();
            _realTrackLabel.AddToClassList("track-timeline-label");
            _realTrackTag = new VisualElement();
            _realTrackTag.AddToClassList("track-timeline-tag");

            _realTrackTimeline.Add(_realTrackLabel);
            _realTrackTimeline.Add(_realTrackTag);

            container.Add(_realTrackTimeline);

            return container; 
        }

        // private void OnPointerDown(PointerDownEvent evt) {
        //     evt.StopPropagation();
        // }

        // private void OnPointerMove(PointerMoveEvent evt) {
        //     evt.StopPropagation();
        // }

        // private void OnPointerUp(PointerUpEvent evt) {
        //     evt.StopPropagation();
        // }

        private void OnTargetChanged(SerializedPropertyChangeEvent evt) {
            if (evt.changedProperty.objectReferenceValue is not Component component) return;
            if (_trackProperty.managedReferenceValue is not MotionTrack track) return;
            if (track.clip.IsValidFor(component)) return;

            track.target = null;
            foreach (var c in component.GetComponents<Component>()) {
                if (track.clip.IsValidFor(c)) {
                    track.target = c;

                    break;
                }
            }
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
                _realTrackLabel.text = clip.name;
            }

            _trackTag.style.backgroundColor = color;
            _realTrackTag.style.backgroundColor = color;

            Repaint();
        }
        
        public void Repaint() {
            _trackTargetContainer.style.width = _parent.leftWidth;
            if (_trackProperty == null) return;

            var track = _trackProperty.managedReferenceValue as MotionTrack;

            _realTrackTimeline.style.width = _parent.pixelsPerSecond * track.settings.duration;
            // _realTrackTimeline.style.left = 

            float left = _parent.minMarginLeft + (_parent.pixelsPerSecond * track.settings.startDelay);
            left -= _parent.startTime * _parent.pixelsPerSecond;

            float right = left + _parent.pixelsPerSecond * (track.settings.duration + track.settings.endDelay);

            if (right < 0 || left > _parent.totalWidth) {
                _trackTimelineContainer.style.display = DisplayStyle.None;
            }
            else {
                _trackTimelineContainer.style.display = DisplayStyle.Flex;
                _realTrackTimeline.style.left = left;
            }
        }
    }
}