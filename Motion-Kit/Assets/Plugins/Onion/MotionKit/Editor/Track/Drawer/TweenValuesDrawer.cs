using UnityEditor;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public abstract class TweenValuesDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var root = new VisualElement();

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(TweenValues<float>))]
    public class TweenFloatValuesDrawer : TweenValuesDrawer {}
}