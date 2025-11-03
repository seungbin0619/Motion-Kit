using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PrimeTween;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006

    public class MotionSequenceView : VisualElement {
        private readonly VisualTreeAsset _trackTemplate;
        private SerializedProperty _sequenceProperty;
        private SerializedProperty _tracksProperty;

        private readonly VisualElement _rootContainer;
        private readonly MotionSequenceTimeRuler _timeRulerContainer;
        private readonly VisualElement _trackListContainer;
        private readonly ListView _trackListView;

        private readonly Button _addTrackButton;
        private readonly Button _removeTrackButton;

        private readonly List<float> _groupStartTime = new();
        private readonly List<int> _groups = new();
        private readonly VisualElement _trackInspectorContainer;


        private readonly VisualElement _separator;
        private bool _isDraggingSeparator = false;
        private float _separatorStartX;
        private float _separatorStartLeftWidth;

        private bool _indicatorScheduled = false;
        private bool _isTrackDraggingDirty = false;
        private bool _isTrackDragging = false;
        private Vector3 _trackDragStartPosition;

        
        private readonly List<int> _sortedSelections = new();
        private readonly Dictionary<int, float> _initialTrackDelays = new();
        private readonly Dictionary<int, float> _initialTrackStartTimes = new();



        private readonly PropertyField _nameField;
        private readonly PropertyField _playOnAwakeField;

        private float _leftWidth = 120f;
        private float _pixelsPerSecond = 60f;
        private float _startTime = 0f;

        public readonly float minMarginLeft = 12f;

        public float totalWidth => _trackListContainer.contentRect.width - leftWidth - 2f;

        public float leftWidth {
            get => _leftWidth;
            private set {
                var clampedValue = Mathf.Clamp(value, 60f, 200f);
                if (Mathf.Approximately(_leftWidth, clampedValue)) return;
                _leftWidth = clampedValue;

                NotifyChange();
            }
        }

        public float pixelsPerSecond {
            get => _pixelsPerSecond;
            set {
                var clampedValue = Mathf.Clamp(value, 32f, 120f);
                if (Mathf.Approximately(_pixelsPerSecond, clampedValue)) return;
                _pixelsPerSecond = clampedValue;

                NotifyChange();
            }
        }

        public float startTime {
            get => _startTime;
            set {
                var clampedValue = Mathf.Max(0f, value);
                if (Mathf.Approximately(_startTime, clampedValue)) return;
                _startTime = clampedValue;

                NotifyChange();
            }
        }

        public MotionSequenceView(VisualTreeAsset template, VisualTreeAsset trackTemplate) {
            if (template == null) return;

            Add(template.CloneTree());
            _trackTemplate = trackTemplate;

            _rootContainer = this.Q<VisualElement>("root-container");
            _rootContainer.Add(_nameField = new() { label = "Name" });
            _rootContainer.Add(_playOnAwakeField = new() { label = "Play On Awake" });

            _trackListContainer = new();
            _trackListContainer.AddToClassList("track-list-container");
            _trackListContainer.RegisterCallback<ClickEvent>(evt => {
                if (evt.target is not VisualElement ve) return;
                if (ve.GetFirstAncestorOfType<MotionTrackView>() != null) return;

                _trackListView.ClearSelection();
            });

            _trackListContainer.RegisterCallback<PointerDownEvent>(OnListPointerDown, TrickleDown.TrickleDown);
            _trackListContainer.RegisterCallback<PointerMoveEvent>(OnListPointerMove, TrickleDown.TrickleDown);
            _trackListContainer.RegisterCallback<PointerUpEvent>(OnListPointerUp, TrickleDown.TrickleDown);

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("track-button-container");
            
            _addTrackButton = new(OnAddButtonClicked);
            _addTrackButton.AddToClassList("track-button");
            _addTrackButton.AddToClassList("add");
            _addTrackButton.text = "+";
            _addTrackButton.tooltip = "Add Track...";
            _addTrackButton.focusable = false;

            _removeTrackButton = new(OnRemoveButtonClicked);
            _removeTrackButton.AddToClassList("track-button");
            _removeTrackButton.AddToClassList("remove");
            _removeTrackButton.text = "-";
            _removeTrackButton.tooltip = "Remove Selected Track(s)";
            _removeTrackButton.SetEnabled(false);
            _removeTrackButton.focusable = false;

            buttonContainer.Add(_addTrackButton);
            buttonContainer.Add(_removeTrackButton);

            _trackListContainer.Add(buttonContainer);
            _trackListContainer.Add(_timeRulerContainer = new(this));
            _trackListContainer.Add(_trackListView = CreateTrackListView());
            _trackListContainer.Add(_separator = CreateSeparator());
            _trackListContainer.RegisterCallback<WheelEvent>(OnWheelZoom, TrickleDown.TrickleDown);

            _rootContainer.Add(_trackListContainer);

            _trackInspectorContainer = new();
            _trackInspectorContainer.AddToClassList("track-inspector-container");
            _rootContainer.Add(_trackInspectorContainer);

            SetSequence(null);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo() {
            if (this == null) return;

            schedule.Execute(() => {
                // secure check for deleted objects after undo/redo
                try {
                    _trackListView.RefreshItems();
                    NotifyChange(); 
                } catch { /* do nothing */ }
            }).ExecuteLater(0);
        }

        private void Repaint() {
            _trackListView.Clear();

            if (_sequenceProperty == null) {
                style.display = DisplayStyle.None;
                return;
            }

            style.display = DisplayStyle.Flex;
            _sequenceProperty.serializedObject.Update();
        }

        private void OnAddButtonClicked() {
            if (_sequenceProperty == null) return;

            var menu = new GenericMenu();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(MotionClip).Name}");

            foreach (var guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<MotionClip>(path);
                if (clip == null) continue;

                var clipType = clip.GetType();
                var menuAttribute = clipType.GetCustomAttribute<MotionClipMenu>();
                
                string menuPath = menuAttribute != null 
                    ? $"{menuAttribute.path}/{clip.name}" 
                    : $"Other/{clip.name}";

                menu.AddItem(new(menuPath), false, OnClipSelected, clip);
            }

            var buttonRect = _addTrackButton.worldBound;
            var menuPosition = new Vector2(buttonRect.xMin, buttonRect.yMax);

            menu.DropDown(new Rect(menuPosition, Vector2.zero));
        }

        private void OnClipSelected(object obj) {
            if (_sequenceProperty == null) return;
            if (obj is not MotionClip clip) return;

            // add new track
            Undo.RecordObject(_sequenceProperty.serializedObject.targetObject, "Add Motion Track");

            int index = _tracksProperty.arraySize;

            _tracksProperty.InsertArrayElementAtIndex(index);
            var trackProp = _tracksProperty.GetArrayElementAtIndex(index);

            Type clipType = clip.GetType(); // clip의 타입 기반으로 MotionTrack 생성
            while (clipType != null && clipType != typeof(object)) {
                if (clipType.IsGenericType && clipType.GetGenericTypeDefinition() == typeof(MotionClipWithValue<>)) {
                    Type genericT = clipType.GetGenericArguments()[0];
                    
                    trackProp.managedReferenceValue = Activator
                        .CreateInstance(typeof(MotionTrack<>)
                        .MakeGenericType(genericT));

                    break;
                }

                clipType = clipType.BaseType;
            }

            trackProp.FindPropertyRelative("clip").objectReferenceValue = clip;

            _sequenceProperty.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveButtonClicked() {
            if (_sequenceProperty == null) return;
            var sortedIndices = _trackListView.selectedIndices.ToList();

            if (sortedIndices.Count == 0) return;
            sortedIndices.Sort();
            sortedIndices.Reverse();

            Undo.RecordObject(_sequenceProperty.serializedObject.targetObject, "Remove Motion Tracks");

            foreach (var index in sortedIndices) {
                _tracksProperty.DeleteArrayElementAtIndex(index);
            }

            _sequenceProperty.serializedObject.ApplyModifiedProperties();
            _trackListView.ClearSelection();
        }

        private ListView CreateTrackListView() {
            var view = new ListView();
            view.AddToClassList("track-list-view");

            view.RegisterCallback<GeometryChangedEvent>(evt => NotifyChange());
            // view.RegisterCallback<FocusOutEvent>(evt => _trackListView.ClearSelection());
            view.RegisterCallback<KeyDownEvent>(OnKeyDown);
            
            view.makeItem = () => {
                schedule.Execute(() => {
                    NotifyChange();
                }).ExecuteLater(0);

                return new MotionTrackView(_trackTemplate, parent: this);
            };

            view.bindItem = (element, index) => {
                var track = element as MotionTrackView;
                var trackProperty = _tracksProperty.GetArrayElementAtIndex(index);

                track.SetTrack(trackProperty, index);
            };

            view.itemIndexChanged += OnTrackIndexChanged;
            view.selectedIndicesChanged += OnTrackSelectionChanged;

            // options
            view.showBoundCollectionSize = false;
            view.allowAdd = false;
            view.allowRemove = false;
            view.reorderable = true;
            view.selectionType = SelectionType.Multiple;
            view.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            
            view.RegisterCallback<AttachToPanelEvent>(evt => {
                var scroll = (evt.target as ListView).Q<ScrollView>();
                
                scroll.mouseWheelScrollSize = 20f;
            });

            return view;
        }

        private void OnListPointerDown(PointerDownEvent evt) {
            if (evt.button != 0) return;
            if (evt.target is not VisualElement element) return;
            if (element is not MotionTrackTimelineView) return;
            if (evt.ctrlKey || evt.shiftKey) return;

            var trackView = element.GetFirstAncestorOfType<MotionTrackView>();
            if (trackView == null) return;

            if (!_trackListView.selectedIndices.Contains(trackView.index)) {
                AddTrackToSelection(trackView.index, additive: false);
            }

            _isTrackDragging = true;
            _isTrackDraggingDirty = false;
            _trackDragStartPosition = evt.position;

            _sortedSelections.Clear();

            foreach (var index in _trackListView.selectedIndices) {
                var trackProp = _tracksProperty.GetArrayElementAtIndex(index);
                var delayProp = trackProp.FindPropertyRelative("settings.startDelay");

                _initialTrackDelays[index] = delayProp.floatValue;
                _initialTrackStartTimes[index] = _groupStartTime[_groups[index]];

                _sortedSelections.Add(index);
            }

            _sortedSelections.Sort((a, b) => {
                var ca = _initialTrackStartTimes[a] + _initialTrackDelays[a];
                var cb = _initialTrackStartTimes[b] + _initialTrackDelays[b];

                return ca.CompareTo(cb);
            });

            evt.StopPropagation();
        }

        private void OnListPointerMove(PointerMoveEvent evt) {
            if (!_isTrackDragging) return;
            float deltaX = evt.position.x - _trackDragStartPosition.x;

            if (!_isTrackDraggingDirty) {
                if (Mathf.Abs(deltaX) < 3f) {
                    return;
                }

                _isTrackDraggingDirty = true;
                Undo.RecordObject(_sequenceProperty.serializedObject.targetObject, "Move Motion Track(s)");
            }

            float deltaTime = deltaX / pixelsPerSecond;
            int _minStartDelayTrack = _sortedSelections[0];

            deltaTime = Mathf.Max(-_initialTrackDelays[_minStartDelayTrack], deltaTime);
            deltaTime = Mathf.Round((_initialTrackDelays[_minStartDelayTrack] + deltaTime) / 0.05f) * 0.05f - 
                _initialTrackDelays[_minStartDelayTrack];

            foreach (var index in _sortedSelections) {
                var trackProp = _tracksProperty.GetArrayElementAtIndex(index);
                var totalDuration = trackProp.managedReferenceValue is MotionTrack track 
                    ? track.totalDuration 
                    : 0f;

                var delayProp = trackProp.FindPropertyRelative("settings.startDelay");
                
                var groupDelta = _initialTrackStartTimes[index] - _groupStartTime[_groups[index]];
                delayProp.floatValue = Mathf.Max(0f, _initialTrackDelays[index] + deltaTime + groupDelta);

                if (_groups[index] >= _groups.Count - 1) continue;
                if (_groupStartTime[_groups[index] + 1] < totalDuration - _initialTrackDelays[index] + delayProp.floatValue) {
                    _groupStartTime[_groups[index] + 1] = totalDuration - _initialTrackDelays[index] + delayProp.floatValue;
                }
            }

            _sequenceProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            NotifyChange();

            evt.StopPropagation();
        }

        private void OnListPointerUp(PointerUpEvent evt) {
            if (evt.button != 0) return;
            if (!_isTrackDragging) return;

            _sequenceProperty.serializedObject.ApplyModifiedProperties();

            _isTrackDragging = false;
            evt.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent evt) {
            bool isDeletePressed = evt.keyCode == KeyCode.Delete;
            if (isDeletePressed && !_isTrackDragging) {
                OnRemoveButtonClicked();
                evt.StopPropagation();
            }
        }

        public void AddTrackToSelection(int index, bool additive = true) {
            if (!additive) {
                _trackListView.ClearSelection();
            }
            
            _trackListView.AddToSelection(index);
        }

        private void OnTrackIndexChanged(int oldIndex, int newIndex) {
            if (_indicatorScheduled) return;

            _indicatorScheduled = true;
            schedule.Execute(() => {
                OnTrackSelectionChanged(_trackListView.selectedIndices);
            }).ExecuteLater(0);
        }

        private void OnTrackSelectionChanged(IEnumerable<int> selectedIndices) {
            _indicatorScheduled = false;

            _removeTrackButton.SetEnabled(selectedIndices.Any());
            RepaintTrackInspector();
        }

        private void RepaintTrackInspector() {
            _trackInspectorContainer.Clear();
            _trackInspectorContainer.style.display = DisplayStyle.None;

            if (_trackListView.selectedIndices.Any()) {
                _trackInspectorContainer.style.display = DisplayStyle.Flex;

                var index = _trackListView.selectedIndices.First();
                var singleTrackProp = _tracksProperty.GetArrayElementAtIndex(index);
                            
                var trackPropertyField = new PropertyField();
                trackPropertyField.BindProperty(singleTrackProp);
                trackPropertyField.RegisterCallback<SerializedPropertyChangeEvent>(evt => {
                    NotifyChange();
                });
                
                _trackInspectorContainer.Add(trackPropertyField);
                return;
            }
        }

        private void OnWheelZoom(WheelEvent evt) {
            if ((evt.ctrlKey || evt.commandKey) && !evt.shiftKey) { 
                float zoomDelta = -evt.delta.y;
                float zoomFactor = 1.1f;

                if (zoomDelta > 0) pixelsPerSecond *= zoomFactor;
                else if (zoomDelta < 0) pixelsPerSecond /= zoomFactor;
                else return;

                evt.StopPropagation();
            }

            if (evt.shiftKey) {
                float scrollDelta = evt.delta.x;
                float scrollFactor = 1.0f / (pixelsPerSecond / 60.0f) * 3f;

                float timeDelta = scrollDelta / pixelsPerSecond * scrollFactor;
                startTime += timeDelta;

                evt.StopPropagation();
            }
        }

        private VisualElement CreateSeparator() {
            var separator = new VisualElement();

            separator.AddToClassList("track-target-separator");
            separator.style.left = leftWidth;

            separator.RegisterCallback<PointerDownEvent>(OnSeparatorPointerDown);
            separator.RegisterCallback<PointerMoveEvent>(OnSeparatorPointerMove);
            separator.RegisterCallback<PointerUpEvent>(OnSeparatorPointerUp);

            return separator;
        }

        private void OnSeparatorPointerDown(PointerDownEvent evt) {
            if (evt.button == 0) {
                _separator.AddToClassList("selected");

                _isDraggingSeparator = true;
                _separatorStartX = evt.position.x;
                _separatorStartLeftWidth = leftWidth;
                _separator.CapturePointer(evt.pointerId); 
                evt.StopPropagation();
            }
        }

        private void OnSeparatorPointerMove(PointerMoveEvent evt) {
            if (_isDraggingSeparator && _separator.HasPointerCapture(evt.pointerId)) {
                float deltaX = evt.position.x - _separatorStartX;
                leftWidth = _separatorStartLeftWidth + deltaX;
                evt.StopPropagation();
            }
        }

        private void OnSeparatorPointerUp(PointerUpEvent evt) {
            if (_isDraggingSeparator && _separator.HasPointerCapture(evt.pointerId)) {
                _separator.RemoveFromClassList("selected");
                
                _isDraggingSeparator = false;
                _separator.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
            }
        }

        private void CalculateTrackStartTimes() {
            float cummulativeTime = 0f;
            _groupStartTime.Clear();
            _groups.Clear();

            for (int i = 0, j; i < _tracksProperty.arraySize; i++) {
                var maxTotalDuration = 0f;
                _groupStartTime.Add(cummulativeTime);

                for(j = i; j < _tracksProperty.arraySize; j++) {
                    var trackProp = _tracksProperty.GetArrayElementAtIndex(j);
                    if (trackProp.managedReferenceValue is not MotionTrack track) break;
                    if (i < j && track.mode == TrackMode.Chain) break;

                    maxTotalDuration = Mathf.Max(maxTotalDuration, track.totalDuration);
                    _groups.Add(_groupStartTime.Count - 1);
                }

                cummulativeTime += maxTotalDuration;
                i = j - 1;
            }
        }

        public void NotifyChange() {
            if (_trackListView == null) return;

            _separator.style.left = _leftWidth;
            _timeRulerContainer.Repaint();

            CalculateTrackStartTimes();

            _trackListView.Query<MotionTrackView>().Visible().ForEach(trackView => {
                trackView.Repaint(_groupStartTime[_groups[trackView.index]]);
            });
        }

        public void SetSequence(SerializedProperty sequenceProperty) {
            if (_sequenceProperty == sequenceProperty) return;

            _nameField.Unbind();
            _playOnAwakeField.Unbind();
            _trackListView.Unbind();
            _sequenceProperty = sequenceProperty;
            
            if (_sequenceProperty != null) {
                _nameField.BindProperty(_sequenceProperty.FindPropertyRelative("name"));
                _playOnAwakeField.BindProperty(_sequenceProperty.FindPropertyRelative("playOnAwake"));

                _tracksProperty = _sequenceProperty.FindPropertyRelative("tracks");
                _trackListView.BindProperty(_tracksProperty);
            }
            
            Repaint();
        }
    }
}