using UnityEditor;
using UnityEngine;
using Screens;
using UnityEngine.UI;

[CustomEditor(typeof(Screen8StringCheck))]
public class Screen8StringCheckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Screen8StringCheck screen8StringCheck = (Screen8StringCheck)target;

        if (GUILayout.Button("Initialize Fields"))
        {
            screen8StringCheck.Init();
        }

        // Display the lists in the Inspector
        if (screen8StringCheck.inputFields != null && screen8StringCheck.inputFields.Count > 0)
        {
            EditorGUILayout.LabelField("Input Fields", EditorStyles.boldLabel);
            foreach (var inputField in screen8StringCheck.inputFields)
            {
                EditorGUILayout.ObjectField(inputField, typeof(InputField), true);
            }
        }

        if (screen8StringCheck.correctChars != null && screen8StringCheck.correctChars.Count > 0)
        {
            EditorGUILayout.LabelField("Correct Chars", EditorStyles.boldLabel);
            foreach (var correctChar in screen8StringCheck.correctChars)
            {
                EditorGUILayout.TextField(correctChar);
            }
        }
    }
}