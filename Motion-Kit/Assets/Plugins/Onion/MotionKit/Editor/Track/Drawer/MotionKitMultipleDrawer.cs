using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Onion.MotionKit.Editor {
    public static class MotionKitMultipleDrawer {
        public static void BindFloatProperties(this FloatField field, SerializedProperty[] properties, string relatedPath, Action onChange = null) {
            if (field.userData is EventCallback<ChangeEvent<float>> existingCallback) {
                field.UnregisterCallback(existingCallback);
                field.userData = null;
            }

            if ((properties?.Length ?? 0) == 0) {
                return;
            }

            var firstValue = properties[0].FindPropertyRelative(relatedPath).floatValue;
            bool isMixed = false;

            foreach (var prop in properties) {
                if (prop.FindPropertyRelative(relatedPath).floatValue != firstValue) {
                    isMixed = true;
                    break;
                }
            }

            if (isMixed) {
                field.showMixedValue = true;
            } else {
                field.value = firstValue;
            }

            EventCallback<ChangeEvent<float>> callback = evt => {
                var serializedObject = properties[0].serializedObject;

                serializedObject.Update();
                foreach (var prop in properties) {
                    prop.FindPropertyRelative(relatedPath).floatValue = evt.newValue;
                }
                
                serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();

                field.showMixedValue = false;
            };

            field.RegisterCallback(callback);
            field.userData = callback;
        }

        public static void BindObjectProperties(this ObjectField field, SerializedProperty[] properties, string relatedPath, Action onChange = null) {
            if (field.userData is EventCallback<ChangeEvent<Object>> existingCallback) {
                field.UnregisterCallback(existingCallback);
                field.userData = null;
            }

            if ((properties?.Length ?? 0) == 0) {
                return;
            }

            var firstValue = properties[0].FindPropertyRelative(relatedPath).objectReferenceValue;
            bool isMixed = false;

            foreach (var prop in properties) {
                if (prop.FindPropertyRelative(relatedPath).objectReferenceValue != firstValue) {
                    isMixed = true;
                    break;
                }
            }

            if (isMixed) {
                field.showMixedValue = true;
            } else {
                field.value = firstValue;
            }

            EventCallback<ChangeEvent<Object>> callback = evt => {
                var serializedObject = properties[0].serializedObject;

                serializedObject.Update();
                foreach (var prop in properties) {
                    prop.FindPropertyRelative(relatedPath).objectReferenceValue = evt.newValue;
                }

                serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();

                field.showMixedValue = false;
            };

            field.RegisterCallback(callback);
            field.userData = callback;
        }

        public static void BindEnumProperties(this EnumField field, SerializedProperty[] properties, string relatedPath, Type enumType, int baseValue = 0, Action onChange = null) {
            if (field.userData is EventCallback<ChangeEvent<Enum>> existingCallback) {
                field.UnregisterCallback(existingCallback);
                field.userData = null;
            }

            if ((properties?.Length ?? 0) == 0) {
                return;
            }

            var firstValue = (Enum)Enum.ToObject(enumType, properties[0].FindPropertyRelative(relatedPath).enumValueIndex + baseValue);
            bool isMixed = false;

            foreach (var prop in properties) {
                var enumValue = (Enum)Enum.ToObject(enumType, prop.FindPropertyRelative(relatedPath).enumValueIndex + baseValue);
                if (!enumValue.Equals(firstValue)) {
                    isMixed = true;
                    break;
                }
            }

            if (isMixed) {
                field.showMixedValue = true;
            } else {
                field.value = firstValue;
            }

            field.Init(firstValue);

            EventCallback<ChangeEvent<Enum>> callback = evt => {
                var serializedObject = properties[0].serializedObject;

                serializedObject.Update();
                foreach (var prop in properties) {
                    prop.FindPropertyRelative(relatedPath).enumValueIndex = Convert.ToInt32(evt.newValue) - baseValue;
                }

                serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();

                field.showMixedValue = false;
            };

            field.RegisterCallback(callback);
            field.userData = callback;
        }

        public static void BindIntegerProperties(this IntegerField field, SerializedProperty[] properties, string relatedPath, Action onChange = null) {
            if (field.userData is EventCallback<ChangeEvent<int>> existingCallback) {
                field.UnregisterCallback(existingCallback);
                field.userData = null;
            }

            if ((properties?.Length ?? 0) == 0) {
                return;
            }

            var firstValue = properties[0].FindPropertyRelative(relatedPath).intValue;
            bool isMixed = false;

            foreach (var prop in properties) {
                if (prop.FindPropertyRelative(relatedPath).intValue != firstValue) {
                    isMixed = true;
                    break;
                }
            }

            if (isMixed) {
                field.showMixedValue = true;
            } else {
                field.value = firstValue;
            }

            EventCallback<ChangeEvent<int>> callback = evt => {
                var serializedObject = properties[0].serializedObject;

                serializedObject.Update();
                foreach (var prop in properties) {
                    prop.FindPropertyRelative(relatedPath).intValue = evt.newValue;
                }

                serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();

                field.showMixedValue = false;
            };

            field.RegisterCallback(callback);
            field.userData = callback;
        }

        public static void BindBoolProperties(this Toggle field, SerializedProperty[] properties, string relatedPath, Action onChange = null) {
            if (field.userData is EventCallback<ChangeEvent<bool>> existingCallback) {
                field.UnregisterCallback(existingCallback);
                field.userData = null;
            }

            if ((properties?.Length ?? 0) == 0) {
                return;
            }

            var firstValue = properties[0].FindPropertyRelative(relatedPath).boolValue;
            bool isMixed = false;

            foreach (var prop in properties) {
                if (prop.FindPropertyRelative(relatedPath).boolValue != firstValue) {
                    isMixed = true;
                    break;
                }
            }

            if (isMixed) {
                field.showMixedValue = true;
            } else {
                field.value = firstValue;
            }

            EventCallback<ChangeEvent<bool>> callback = evt => {
                var serializedObject = properties[0].serializedObject;

                serializedObject.Update();
                foreach (var prop in properties) {
                    prop.FindPropertyRelative(relatedPath).boolValue = evt.newValue;
                }

                serializedObject.ApplyModifiedProperties();
                onChange?.Invoke();

                field.showMixedValue = false;
            };

            field.RegisterCallback(callback);
            field.userData = callback;
        }
    }
}