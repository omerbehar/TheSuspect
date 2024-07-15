using Screens;
using UnityEditor;
using UnityEngine;
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

        if (GUILayout.Button("Clear Fields"))
        {
            screen8StringCheck.ClearFields();
        }


    }
}
