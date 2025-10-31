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

        private readonly HashSet<SerializedProperty> _selectedTrackProperties = new();
        private readonly VisualElement _trackInspectorContainer;

        private readonly VisualElement _separator;
        private bool _isDraggingSeparator = false;
        private float _separatorStartX;
        private float _separatorStartLeftWidth;

        private readonly PropertyField _nameField;

        private float _leftWidth = 120f;
        private float _pixelsPerSecond = 60f;
        private float _startTime = 0f;

        public readonly float minMarginLeft = 12f;

        public float leftWidth {
            get => _leftWidth;
            private set {
                var clampedValue = Mathf.Clamp(value, 60f, 200f);
                if (Mathf.Approximately(_leftWidth, clampedValue)) return;
                _leftWidth = clampedValue;

                NotifyGeometryChange();
            }
        }

        public float pixelsPerSecond {
            get => _pixelsPerSecond;
            set {
                var clampedValue = Mathf.Clamp(value, 32f, 120f);
                if (Mathf.Approximately(_pixelsPerSecond, clampedValue)) return;
                _pixelsPerSecond = clampedValue;

                NotifyGeometryChange();
            }
        }

        public float startTime {
            get => _startTime;
            set {
                var clampedValue = Mathf.Max(0f, value);
                if (Mathf.Approximately(_startTime, clampedValue)) return;
                _startTime = clampedValue;

                NotifyGeometryChange();
            }
        }

        public MotionSequenceView(VisualTreeAsset template, VisualTreeAsset trackTemplate) {
            if (template == null) return;

            Add(template.CloneTree());
            _trackTemplate = trackTemplate;

            _rootContainer = this.Q<VisualElement>("root-container");
            _rootContainer.Add(_nameField = new() { label = "Name" });

            _trackListContainer = new();
            _trackListContainer.AddToClassList("track-list-container");
            
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("track-button-container");
            
            _addTrackButton = new(OnAddButtonClicked);
            _addTrackButton.AddToClassList("track-button");
            _addTrackButton.text = "+";
            _addTrackButton.tooltip = "Add Track...";
            _addTrackButton.focusable = false;

            _removeTrackButton = new(OnRemoveButtonClicked);
            _removeTrackButton.AddToClassList("track-button");
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
            if (_selectedTrackProperties.Count == 0) return;

            var selectedIndices = _trackListView.selectedIndices.ToList();

            selectedIndices.Sort();
            selectedIndices.Reverse();

            Undo.RecordObject(_sequenceProperty.serializedObject.targetObject, "Remove Motion Tracks");

            foreach (var index in selectedIndices) {
                _tracksProperty.DeleteArrayElementAtIndex(index);
            }

            _sequenceProperty.serializedObject.ApplyModifiedProperties();
            _trackListView.ClearSelection();
        }

        private ListView CreateTrackListView() {
            var view = new ListView();
            view.AddToClassList("track-list-view");

            view.RegisterCallback<GeometryChangedEvent>(evt => NotifyGeometryChange());
            view.RegisterCallback<KeyDownEvent>(OnKeyDown);
            
            view.makeItem = () => new MotionTrackView(_trackTemplate, parent: this);
            view.bindItem = (element, index) => {
                var track = element as MotionTrackView;
                var trackProperty = _tracksProperty.GetArrayElementAtIndex(index);

                track.SetTrack(trackProperty);
            };

            view.selectionChanged += OnTrackSelectionChanged;

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

        private void OnKeyDown(KeyDownEvent evt) {
            bool isDeletePressed = evt.keyCode == KeyCode.Delete;
            if (isDeletePressed) {
                
                OnRemoveButtonClicked();
                evt.StopPropagation();
            }
        }

        private void OnTrackSelectionChanged(IEnumerable<object> selectedItems) {
            _selectedTrackProperties.Clear();

            foreach (var item in selectedItems) {
                if (item is SerializedProperty prop) {
                    _selectedTrackProperties.Add(prop);
                }
            }

            _removeTrackButton.SetEnabled(_selectedTrackProperties.Count > 0);
            RepaintTrackInspector();
        }

        private void RepaintTrackInspector() {
            _trackInspectorContainer.Clear();
            _trackInspectorContainer.style.display = DisplayStyle.None;

            if (_selectedTrackProperties.Count == 1) {
                _trackInspectorContainer.style.display = DisplayStyle.Flex;
                
                var singleTrackProp = _selectedTrackProperties.First();                
                var trackPropertyField = new PropertyField();
                trackPropertyField.BindProperty(singleTrackProp);
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

        private void NotifyGeometryChange() {
             if (_trackListView == null) return;

             _separator.style.left = _leftWidth;
             _timeRulerContainer.Repaint();

            _trackListView.Query<MotionTrackView>().Visible().ForEach(trackView => {
                 trackView.Repaint();
            });
        }

        public void SetSequence(SerializedProperty sequenceProperty) {
            if (_sequenceProperty == sequenceProperty) return;

            _nameField.Unbind();
            _trackListView.Unbind();
            _sequenceProperty = sequenceProperty;
            
            if (_sequenceProperty != null) {
                _nameField.BindProperty(_sequenceProperty.FindPropertyRelative("name"));

                _tracksProperty = _sequenceProperty.FindPropertyRelative("tracks");
                _trackListView.BindProperty(_tracksProperty);
            }
            
            Repaint();
        }
    }
}