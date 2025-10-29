using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public class MotionSequenceView : VisualElement {
        private readonly VisualTreeAsset _trackTemplate;
        private SerializedProperty _sequenceProperty;

        private readonly VisualElement _rootContainer;
        private readonly ListView _trackListView;
        private readonly PropertyField _nameField;

        public MotionSequenceView(VisualTreeAsset template, VisualTreeAsset trackTemplate) {
            if (template == null) return;

            Add(template.CloneTree());
            _trackTemplate = trackTemplate;

            _rootContainer = this.Q<VisualElement>("root-container");
            
            _nameField = new() { label = "Name" };
            _trackListView = CreateTrackListView();

            _rootContainer.Add(_nameField);
            _rootContainer.Add(_trackListView);

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
            
            view.makeItem = () => new MotionTrackView(_trackTemplate);
            view.bindItem = (element, i) => {
                var track = element as MotionTrackView;
                var trackProperty = _sequenceProperty.FindPropertyRelative("tracks").GetArrayElementAtIndex(i);

                track.SetTrack(trackProperty);
            };

            // options
            view.showBoundCollectionSize = false;
            view.reorderable = true;
            view.selectionType = SelectionType.Multiple;
            view.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            return view;
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