using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006
    public class MotionSequenceTimeRuler : VisualElement {
        private readonly MotionSequenceView _parent;
        private readonly VisualElement _mainContainer;
        private readonly VisualElement _signalContainer;
        private readonly List<int> _selectedIndices = new();
        public List<int> selectedIndices => _selectedIndices;
        private readonly Dictionary<int, MotionSignalView> _signalViews = new();

        public MotionSequenceTimeRuler(MotionSequenceView parent = null) {
            _parent = parent;
            
            Add(_mainContainer = new VisualElement());
            Add(_signalContainer = new VisualElement());

            _mainContainer.AddToClassList("time-ruler-main-container");
            _signalContainer.AddToClassList("time-ruler-signal-container");

            AddToClassList("time-ruler-container");
            
            RegisterCallback<ClickEvent>(OnClick);
        }

        public void OnClick(ClickEvent evt) {
            evt.StopPropagation();
            _parent.ClearTrackSelection();
            
            if (evt.target is MotionSignalView) {
                return;
            }

            ClearSelection();
        }

        public void Repaint(SerializedProperty property = null) {
            _mainContainer.Clear();
            if (_parent == null) return;

            style.marginLeft = _parent.leftWidth + 2;

            float second = Mathf.Ceil(_parent.startTime * 2.0f) * 0.5f - 1;
            float position = _parent.minMarginLeft + (second - _parent.startTime) * _parent.pixelsPerSecond;

            while (position < _parent.totalWidth) {
                if (second % 1.0f == 0) {
                    var tick = new VisualElement();
                    tick.AddToClassList("time-ruler-tick");
                    tick.style.left = position;
                    _mainContainer.Add(tick);

                    var label = new Label($"{second}");
                    label.AddToClassList("time-ruler-label");
                    label.style.left = position;
                    _mainContainer.Add(label);
                } else {
                    var halfTick = new VisualElement();
                    halfTick.AddToClassList("time-ruler-tick");
                    halfTick.AddToClassList("half-tick");
                    halfTick.style.left = position;
                    _mainContainer.Add(halfTick);
                }

                second += 0.5f;
                position += _parent.pixelsPerSecond * 0.5f;
            }

            float from = _parent.startTime - _parent.minMarginLeft / _parent.pixelsPerSecond;
            float to = from + _parent.totalWidth / _parent.pixelsPerSecond;

            if (property == null) return;

            // _signalContainer.Clear();

            for (int i = 0; i < property.arraySize; i++) {
                var signalProperty = property.GetArrayElementAtIndex(i);
                float signalTime = signalProperty.FindPropertyRelative("time").floatValue;

                if (signalTime < from || signalTime > to) {
                    if (_signalViews.ContainsKey(i)) {
                        var existingSignal = _signalViews[i];

                        existingSignal.style.display = DisplayStyle.None;
                    }

                    continue;
                }

                float signalPosition = _parent.minMarginLeft + (signalTime - _parent.startTime) * _parent.pixelsPerSecond;

                if (_signalViews.ContainsKey(i)) {
                    var existingSignal = _signalViews[i];

                    existingSignal.Initialize(i);
                    existingSignal.Repaint(signalPosition);
                    existingSignal.style.display = DisplayStyle.Flex;
                    
                    continue;
                }

                var signalMarker = new MotionSignalView(this);

                signalMarker.Initialize(i);
                signalMarker.Repaint(signalPosition);
                signalMarker.style.display = DisplayStyle.Flex;

                _signalViews[i] = signalMarker;
                _signalContainer.Add(signalMarker);
            }
        }

        public void AddToSelection(MotionSignalView signal, bool additive) {
            if (!additive) {
                ClearSelection();
            }

            if (!_selectedIndices.Contains(signal.index)) {
                _selectedIndices.Add(signal.index);
            }

            _parent.RepaintTrackInspector();
        }

        public void RemoveFromSelection(MotionSignalView signal) {
            if (_selectedIndices.Contains(signal.index)) {
                _selectedIndices.Remove(signal.index);
            }

            _parent.RepaintTrackInspector();
        }

        public void ClearSelection() {
            foreach (var index in _selectedIndices) {
                if (_signalViews.ContainsKey(index)) {
                    _signalViews[index].Deselect();
                }
            }

            _selectedIndices.Clear();
            _parent.RepaintTrackInspector();
        }
    }
}