using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
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

        private static Texture2D _endDelayTex = null;
        private static Texture2D endDelayTex {
            get {
                if (_endDelayTex == null) {
                    int size = 18;
                    _endDelayTex = new(size, size, TextureFormat.RGBA32, false) {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    Color clear = new(1, 1, 1, 0);
                    Color line = new(1, 1, 1, 0.2f);

                    for (int y = 0; y < size; y++) {
                        for (int x = 0; x < size; x++) {
                            bool isLine = (x - y + size) % 6 < 2;

                            _endDelayTex.SetPixel(x, y, isLine ? line : clear);
                        }
                    }
                    
                    _endDelayTex.Apply();
                }

                return _endDelayTex;
            }
        }

        private SerializedProperty _trackProperty;
        public SerializedProperty trackProperty => _trackProperty;

        private readonly VisualElement _container;
        private readonly MotionSequenceView _parent;

        private readonly VisualElement _trackTargetContainer;
        private VisualElement _trackTag;
        private PropertyField _trackTargetField;
        
        private VisualElement _trackTimelineContainer;
        private MotionTrackTimelineView _realTrackTimeline;
        private VisualElement _realTrackDurationContent;
        private Label _realTrackLabel;
        private VisualElement _realTrackTag;

        private bool _isResizing = false;
        private int _index;
        public int index => _index;
        
        private bool _isDraggingDirty = false;
        private Vector2 _dragStartPosition;
        private float _originalStartDelay;
        private float _originalDuration;
        private SerializedProperty _resizingDurationProp;
        private SerializedProperty _resizingStartDelayProp;

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
            container.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            container.RegisterCallback<PointerUpEvent>(OnPointerUp);

            _realTrackTimeline = new MotionTrackTimelineView();
            _realTrackTimeline.AddToClassList("track-timeline-content");

            _realTrackDurationContent = new();
            _realTrackDurationContent.AddToClassList("track-timeline-duration-content");

            _realTrackLabel = new Label();
            _realTrackLabel.pickingMode = PickingMode.Ignore;
            _realTrackLabel.AddToClassList("track-timeline-label");
            
            _realTrackTag = new VisualElement();
            _realTrackTag.pickingMode = PickingMode.Ignore;
            _realTrackTag.AddToClassList("track-timeline-tag");

            _realTrackTimeline.Add(_realTrackDurationContent);
            _realTrackTimeline.style.backgroundImage = endDelayTex;

            _realTrackDurationContent.Add(_realTrackLabel);
            _realTrackDurationContent.Add(_realTrackTag);


            var leftResizeHandle = new VisualElement { 
                name = "left-resize-handle", 
                style = { left = 0 }
            };
            leftResizeHandle.AddToClassList("track-timeline-resize-handle");
            leftResizeHandle.RegisterCallback<PointerDownEvent>(OnPointerDown);

            var rightResizeHandle = new VisualElement { 
                name = "right-resize-handle", 
                style = { right = 0 } 
            };
            rightResizeHandle.AddToClassList("track-timeline-resize-handle");
            rightResizeHandle.RegisterCallback<PointerDownEvent>(OnPointerDown);

            _realTrackTimeline.Add(leftResizeHandle);
            _realTrackTimeline.Add(rightResizeHandle);

            container.Add(_realTrackTimeline);

            return container; 
        }

        private void OnPointerDown(PointerDownEvent evt) {
            if (evt.target is not VisualElement element) return;
            if (_trackProperty == null) return;

            _resizingStartDelayProp = _trackProperty.FindPropertyRelative("settings.startDelay");
            _resizingDurationProp = _trackProperty.FindPropertyRelative("settings.duration");

            if (_resizingDurationProp == null) return;
            if (_resizingStartDelayProp == null) return;

            _isResizing = true;
            _dragStartPosition = evt.position;
            _originalStartDelay = _resizingStartDelayProp.floatValue;
            _originalDuration = _resizingDurationProp.floatValue;

            if (element.name == "left-resize-handle") {
                /* do nothing */
            } else if (element.name == "right-resize-handle") {
                _resizingStartDelayProp = null;
            } else {
                _isResizing = false;
                return;
            }

            _trackTimelineContainer.CapturePointer(evt.pointerId);
            _parent.AddTrackToSelection(_index, false);
            
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt) {
            if (!_isResizing || _resizingDurationProp == null) return;

            float delta = evt.position.x - _dragStartPosition.x;
            if (!_isDraggingDirty) {
                if (Mathf.Abs(delta) < 3f) return;

                _isDraggingDirty = true;
                Undo.RecordObject(_trackProperty.serializedObject.targetObject, "Resize Motion Track");
            }

            float deltaTime = delta / _parent.pixelsPerSecond;

            if (_resizingStartDelayProp == null) {
                var result = Mathf.Max(0f, _originalDuration + deltaTime);
                if (result > 0f) result = Mathf.Round(result / 0.05f) * 0.05f;
                if (result == 0f) result = 0.01f;

                _resizingDurationProp.floatValue = result;
            } else {
                deltaTime = Mathf.Clamp(deltaTime, -_originalStartDelay, _originalDuration);
                var result = Mathf.Max(0f, _originalStartDelay + deltaTime);

                if (result > 0f) result = Mathf.Round(result / 0.05f) * 0.05f;

                _resizingStartDelayProp.floatValue = result;
                _resizingDurationProp.floatValue = Mathf.Max(0.01f, _originalDuration - (result - _originalStartDelay));
            }

            _trackProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            _parent.NotifyChange();

            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt) {
            if (!_isResizing) return;

            _isResizing = false;
            _resizingDurationProp = null;
            _trackTimelineContainer.ReleasePointer(evt.pointerId);

            _trackProperty.serializedObject.ApplyModifiedProperties();
            evt.StopPropagation();
        }

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

        public void SetTrack(SerializedProperty trackProperty, int index) {
            if (_trackProperty == trackProperty) return;

            _trackTargetField.Unbind();
            _trackProperty = trackProperty;
            _index = index;

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
        }
        
        public void Repaint(float groupStartTime = 0.0f) {
            if (this == null) return;
            
            _trackTargetContainer.style.width = _parent.leftWidth;
            if (_trackProperty == null) return;
            if (_trackProperty.managedReferenceValue is not MotionTrack track) return;

            _realTrackTimeline.style.width = _parent.pixelsPerSecond * (track.settings.duration + track.settings.endDelay);
            _realTrackDurationContent.style.width = _parent.pixelsPerSecond * track.settings.duration;

            float left = _parent.minMarginLeft + _parent.pixelsPerSecond * (track.settings.startDelay + groupStartTime);
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

    public class MotionTrackTimelineView : VisualElement{ }
}