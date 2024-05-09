using UnityEngine;
using UnityEditor;

#if FALSE
[CustomEditor(typeof(InfoTile))]
public class EditorInfoTile : Editor
{
    private SerializedProperty _guid;

    public override void OnInspectorGUI()
    {
        // Cache the serialized object to avoid calling it multiple times
        SerializedObject serializedObject = this.serializedObject;

        // Ensure the serialized object is updated before accessing its properties
        serializedObject.Update();

        InfoTile tile = (InfoTile)target;

        // Use EditorGUILayout.BeginHorizontal to align the button and the property field
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            tile.AssignObject();
        }

        // Use EditorGUILayout.PropertyField to display the property field
        EditorGUILayout.PropertyField(_guid, new GUIContent("Loadable ID"), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        // Apply the modified properties to the serialized object
        serializedObject.ApplyModifiedProperties();

        // Draw the default inspector after your custom GUI
        DrawDefaultInspector();
    }

    // Move the OnEnable method outside of OnInspectorGUI and make it private
    private void OnEnable()
    {
        // Find the serialized property for _guid
        _guid = serializedObject.FindProperty("_guid");
    }
}
#endif
