using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006

    public class MotionSequenceView : VisualElement {
        private readonly VisualTreeAsset _trackTemplate;
        private SerializedProperty _sequenceProperty;

        private readonly VisualElement _rootContainer;
        private readonly MotionSequenceTimeRuler _timeRulerContainer;
        private readonly VisualElement _trackListContainer;
        private readonly ListView _trackListView;


        private readonly VisualElement _separator;
        private bool _isDraggingSeparator = false;
        private float _separatorStartX;
        private float _separatorStartLeftWidth;


        private readonly PropertyField _nameField;

        private float _leftWidth = 120f;
        private float _pixelPerSecond = 60f;

        public readonly float minMarginLeft = 12f;

        public float leftWidth {
            get => _leftWidth;
            private set {
                _leftWidth = Mathf.Clamp(value, 60f, 200f);

                NotifyGeometryChange();
            }
        }

        public float pixelPerSecond {
            get => _pixelPerSecond;
            set {
                _pixelPerSecond = Mathf.Max(20f, value);

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
            _trackListContainer.Add(_timeRulerContainer = new(this));
            _trackListContainer.Add(_trackListView = CreateTrackListView());
            _trackListContainer.Add(_separator = CreateSeparator());

            _rootContainer.Add(_trackListContainer);

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

        private ListView CreateTrackListView() {
            var view = new ListView();
            view.AddToClassList("track-list-view");
            view.RegisterCallback<GeometryChangedEvent>(evt => NotifyGeometryChange());
            
            view.makeItem = () => new MotionTrackView(_trackTemplate, parent: this);
            view.bindItem = (element, index) => {
                var track = element as MotionTrackView;
                var trackProperty = _sequenceProperty.FindPropertyRelative("tracks").GetArrayElementAtIndex(index);

                track.SetTrack(trackProperty);
            };

            // options
            view.showBoundCollectionSize = false;
            view.reorderable = true;
            view.selectionType = SelectionType.Multiple;
            view.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            return view;
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

                var tracksProperty = _sequenceProperty.FindPropertyRelative("tracks");
                _trackListView.BindProperty(tracksProperty);
            }
            
            Repaint();
        }
    }
}