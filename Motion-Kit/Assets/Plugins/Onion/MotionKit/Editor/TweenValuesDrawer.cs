using UnityEditor;
using UnityEngine;

namespace Onion.MotionKit.Editor {
    public abstract class TweenValuesDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var startFromCurrentProp = property.FindPropertyRelative("startFromCurrent");
            var startValueProp = property.FindPropertyRelative("startValue");
            var endValueProp = property.FindPropertyRelative("endValue");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            var line1 = new Rect(position.x, position.y, position.width, lineHeight);
            var line2 = new Rect(position.x, line1.yMax + spacing, position.width, lineHeight);
            var line3 = new Rect(position.x, line2.yMax + spacing, position.width, lineHeight);

            EditorGUI.PropertyField(line1, startFromCurrentProp); 

            if (startFromCurrentProp.boolValue){
                EditorGUI.PropertyField(line2, endValueProp);
            }
            else {
                EditorGUI.PropertyField(line2, startValueProp);
                EditorGUI.PropertyField(line3, endValueProp);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var startFromCurrentProp = property.FindPropertyRelative("startFromCurrent");
            float lineCount;
            
            if (startFromCurrentProp != null && !startFromCurrentProp.boolValue) {
                lineCount = 3; 
            }
            else {
                lineCount = 2;
            }

            float height = (EditorGUIUtility.singleLineHeight * lineCount) + 
                        (EditorGUIUtility.standardVerticalSpacing * (lineCount - 1));
            
            return height;
        }
    }

    [CustomPropertyDrawer(typeof(TweenValues<int>))]
    public sealed class IntTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<long>))]
    public sealed class LongTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<float>))]
    public sealed class FloatTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<double>))]
    public sealed class DoubleTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Vector2>))]
    public sealed class Vector2TweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Vector2Int>))]
    public sealed class Vector2IntTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Vector3>))]
    public sealed class Vector3TweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Vector3Int>))]
    public sealed class Vector3IntTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Vector4>))]
    public sealed class Vector4TweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Rect>))]
    public sealed class RectTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Color>))]
    public sealed class ColorTweenValuesDrawer : TweenValuesDrawer { }

    [CustomPropertyDrawer(typeof(TweenValues<Quaternion>))]
    public sealed class QuaternionTweenValuesDrawer : TweenValuesDrawer { }
}