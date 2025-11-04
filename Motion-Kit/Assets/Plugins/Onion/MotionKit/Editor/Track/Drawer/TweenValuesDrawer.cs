using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Onion.MotionKit.Editor {
    public abstract class TweenValuesDrawer : PropertyDrawer {
        private PropertyField _startFromCurrentField;
        private PropertyField _startValueField;
        private PropertyField _endValueField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            var root = new VisualElement();

            _startFromCurrentField = new PropertyField(property.FindPropertyRelative("startFromCurrent"));
            _startValueField = new PropertyField(property.FindPropertyRelative("startValue"));
            _endValueField = new PropertyField(property.FindPropertyRelative("endValue"));

            root.Add(_startFromCurrentField);
            _startFromCurrentField.RegisterCallback<SerializedPropertyChangeEvent>(evt => {
                var prop = evt.changedProperty;
                bool useCurrent = prop.boolValue;
                
                _startValueField.style.display = useCurrent 
                    ? DisplayStyle.None 
                    : DisplayStyle.Flex;
            });

            root.Add(_startValueField);
            root.Add(_endValueField);

            return root;
        }
    }

    // available types - int, long, float, double, Vector2, Vector3, Vector4, Color, Quaternion, Rect

    [CustomPropertyDrawer(typeof(TweenValues<float>))]
    public class TweenFloatValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<int>))]
    public class TweenIntValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<long>))]
    public class TweenLongValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<double>))]
    public class TweenDoubleValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<Vector2>))]
    public class TweenVector2ValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<Vector3>))]
    public class TweenVector3ValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<Color>))]
    public class TweenColorValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<Quaternion>))]
    public class TweenQuaternionValuesDrawer : TweenValuesDrawer {}

    [CustomPropertyDrawer(typeof(TweenValues<Rect>))]
    public class TweenRectValuesDrawer : TweenValuesDrawer {}
}