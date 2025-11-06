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
        public static Texture2D endDelayTex {
            get {
                if (_endDelayTex == null) {
                    int size = 18;
                    _endDelayTex = new(size, size, TextureFormat.RGBA32, false) {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    Color clear = new(1, 1, 1, 0);
                    Color line = new(1, 1, 1, 0.1f);

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
        
        private readonly VisualElement _trackTimelineContainer;
        private MotionTrackTimelineView _realTrackTimeline;
        private VisualElement _cyclesContainer;
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

        private readonly List<MotionTrackCycleView> _cyclePool = new();

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
            _realTrackDurationContent.pickingMode = PickingMode.Ignore;

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

            _cyclesContainer = new VisualElement();
            _cyclesContainer.AddToClassList("track-timeline-cycles-container");
            _cyclesContainer.pickingMode = PickingMode.Ignore;

            container.Add(_cyclesContainer);

            return container; 
        }

        private void OnPointerDown(PointerDownEvent evt) {
            if (evt.target is not VisualElement element) return;
            if (_trackProperty == null) return;

            _resizingStartDelayProp = _trackProperty.FindPropertyRelative("delay");
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
            if (_trackProperty.managedReferenceValue is not MotionTrack track) return;
            if (track.clip == null) return;

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

            _realTrackTimeline.tooltip = _realTrackLabel.text;
            _trackTag.style.backgroundColor = color;
            _realTrackTag.style.backgroundColor = color;
        }
        
        public void Repaint(float groupStartTime = 0.0f) {
            if (this == null) return;
            if (groupStartTime == float.PositiveInfinity) {
                _trackTimelineContainer.style.display = DisplayStyle.None;
                _container.style.opacity = 0.5f;

                return;
            }

            _container.style.opacity = 1.0f;
            _trackTargetContainer.style.width = _parent.leftWidth;
            
            if (_trackProperty == null) return;
            if (_trackProperty.managedReferenceValue is not MotionTrack track) return;

            float durationWidth = _parent.pixelsPerSecond * track.settings.duration;
            float startDelayWidth = _parent.pixelsPerSecond * track.settings.startDelay;
            float timelineWidth = durationWidth + startDelayWidth + _parent.pixelsPerSecond * track.settings.endDelay;

            bool isInfinite = track.settings.cycles == -1;

            int cycles = Mathf.Max(track.settings.cycles, 1);
            float totalWidth = isInfinite ? float.PositiveInfinity : timelineWidth * cycles;

            _realTrackTimeline.style.width = timelineWidth;
            _realTrackDurationContent.style.width = durationWidth;

            float left = _parent.minMarginLeft + _parent.pixelsPerSecond * (track.delay + groupStartTime - _parent.startTime);
            float right = left + totalWidth;

            if (right < 0 || left > _parent.totalWidth) {
                _trackTimelineContainer.style.display = DisplayStyle.None;
                return;
            }

            _trackTimelineContainer.style.display = DisplayStyle.Flex;

            int skipTracks = Mathf.Max(0, Mathf.CeilToInt(-left / timelineWidth - 1));
            float position = left + skipTracks * timelineWidth;

            if (skipTracks == 0) {
                _realTrackTimeline.style.display = DisplayStyle.Flex;
                _realTrackTimeline.style.left = position;
                _realTrackDurationContent.style.left = startDelayWidth;
                
                position += timelineWidth;
                skipTracks = 1;
            } else {
                _realTrackTimeline.style.display = DisplayStyle.None;
            }

            int visibleCycles = Mathf.CeilToInt((_parent.totalWidth - position) / timelineWidth);
            if (!isInfinite) {
                visibleCycles = Mathf.Min(visibleCycles, cycles - skipTracks);
            }

            visibleCycles = Mathf.Max(visibleCycles, 0);

            _cyclesContainer.style.display = visibleCycles > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _cyclesContainer.style.left = position;

            while (_cyclePool.Count < visibleCycles) {
                var content = new MotionTrackCycleView(_realTrackLabel.text, _realTrackTag.style.backgroundColor.value);
                _cyclePool.Add(content);
            }

            while (_cyclesContainer.childCount < visibleCycles) {
                _cyclesContainer.Add(_cyclePool[_cyclesContainer.childCount]);
            }

            while (_cyclesContainer.childCount > visibleCycles) {
                var lastIndex = _cyclesContainer.childCount - 1;

                _cyclesContainer.Remove(_cyclesContainer[lastIndex]);
            }

            foreach (var cycleView in _cyclePool) {
                cycleView.Repaint(timelineWidth, durationWidth, startDelayWidth);
            }
        }
    }

    public class MotionTrackTimelineView : VisualElement{ }

    public class MotionTrackCycleView : VisualElement {
        private readonly VisualElement _root;
        private readonly VisualElement _content;

        public MotionTrackCycleView(string text, StyleColor color) {
            _root = new VisualElement();
            _content = new VisualElement();

            var tag = new VisualElement();
            var label = new Label();

            _root.AddToClassList("track-timeline-cycle-bg");
            _root.style.backgroundImage = MotionTrackView.endDelayTex;

            _content.AddToClassList("track-timeline-cycle-content");

            tag.AddToClassList("track-timeline-tag");
            tag.style.backgroundColor = color;

            label.text = text;
            label.AddToClassList("track-timeline-label");

            _content.Add(tag);
            _content.Add(label);
            _root.Add(_content);

            Add(_root);
        }

        public void Repaint(float totalWidth, float durationWidth, float startDelay) {
            _root.style.width = totalWidth;
            _content.style.width = durationWidth;
            _content.style.left = startDelay;
        }
    }
}