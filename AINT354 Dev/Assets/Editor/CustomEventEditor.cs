using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// Some code taken from https://stackoverflow.com/questions/56180821/how-can-i-use-reorderablelist-with-a-list-in-the-inspector-and-adding-new-empty
[CustomEditor(typeof(CustomEventTrigger))]
public class CustomEventEditor : Editor
{

    private ReorderableList data;

    private void OnEnable()
    {
        data = new ReorderableList(serializedObject, serializedObject.FindProperty("triggerList")) {

            displayAdd = true,
            displayRemove = true,
            draggable = true,

            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Triggers");
            },

            onAddCallback = (list) =>
            {
                list.serializedProperty.arraySize++;
                SerializedProperty addedElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);

                var foldout = addedElement.FindPropertyRelative("foldout");
                foldout.boolValue = true;
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = data.serializedProperty.GetArrayElementAtIndex(index);

                rect.x += 10;
                rect.y += 2;

                var position = new Rect(rect);
                var foldout = element.FindPropertyRelative("foldout");
                foldout.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, 10, EditorGUIUtility.singleLineHeight), foldout.boolValue, foldout.boolValue ? "" : 
                    element.FindPropertyRelative("triggerName").stringValue != "" ? element.FindPropertyRelative("triggerName").stringValue : ("Method \'" + element.FindPropertyRelative("methodName").stringValue + "\' on tag \'" + element.FindPropertyRelative("tag").stringValue + "\'"));

                if (foldout.boolValue)
                {
                    rect.y += 2;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("triggerName"), new GUIContent("Trigger Name", "An optional name for this trigger, to call it more easily (but slowly) from a script."));

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("scriptName"), new GUIContent("Script Name", "The name of the C# Script you are going to call a method on."));

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("methodName"), new GUIContent("Method Name", "The method to call on the Handler. If parameters are required, these need to be set through code (see provided examples)."));

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("tag"), new GUIContent("Tag", "The tag for the Custom Event Handler object. Case-insensitive."));

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("enableInactiveObjects"), new GUIContent("Enable Inactive", "Enables GameObjects that are not set as active. This will not actiavte parent objects, so the object will remain inactive if its parents are not already active."));

                    rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("exactTagMatch"), new GUIContent("Exact Tag Match", "If true, the handler's tag will need to match the provided text fully. If false, the handler's tag will only need to contain the provided text."));
                }
            },

            elementHeightCallback = (index) =>
            {
                var element = data.serializedProperty.GetArrayElementAtIndex(index);
                if (element.FindPropertyRelative("foldout").boolValue)
                {
                    return ((EditorGUIUtility.singleLineHeight * 1.25f) * 6) + 2; //6 lines of 1.25x line height, with 2px padding at the bottom
                }
                else
                {
                    return (EditorGUIUtility.singleLineHeight * 1.25f) + 2; //Single line with bottom padding
                }
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        data.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
