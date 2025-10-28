using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit {
    [CustomEditor(typeof(MotionAnimator))]
    public class MotionAnimatorEditor : Editor {
        [SerializeField]
        private VisualTreeAsset _template;
        
        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            return root;
        }
    }
}