using UnityEditor;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    #pragma warning disable IDE1006
    
    public class MotionSignalView : VisualElement {
        private readonly MotionSequenceTimeRuler _parent;
        private bool _isSelected = false;
        private int _index = -1;
        public int index => _index;

        public MotionSignalView(MotionSequenceTimeRuler parent) {
            _parent = parent;
            RegisterCallback<ClickEvent>(OnClick);

            focusable = true;
            AddToClassList("motion-signal");
        }

        private void OnClick(ClickEvent evt) {
            Select(evt.ctrlKey);
        }

        public void Initialize(int index) {
            if (index < 0) return;
            if (index == _index) return;

            _index = index;
            Deselect();
        }

        public void Repaint(float position) {
            style.left = position;
        }

        public void Select(bool additive) {
            if (_isSelected) {
                if (additive) {
                    _isSelected = false;
                    RemoveFromClassList("selected");
                    
                    _parent.RemoveFromSelection(this);
                }

                return;
            }
            
            _isSelected = true;
            _parent.AddToSelection(this, additive);

            AddToClassList("selected");
        }

        public void Deselect() {
            if (!_isSelected) return;

            _isSelected = false;
            RemoveFromClassList("selected");
        }
    }
}