using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    [CustomEditor(typeof(MotionAnimator))]
    public class MotionAnimatorEditor : UnityEditor.Editor {
        [SerializeField]
        private VisualTreeAsset _template;
        
        // public override VisualElement CreateInspectorGUI() {
        //     VisualElement root = new();
        //     InspectorElement.FillDefaultInspector(root, serializedObject, this);

        //     return root;
        // }
    }
}