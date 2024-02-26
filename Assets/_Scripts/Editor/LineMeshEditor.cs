using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LineMesh))]
public class LineMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LineMesh lm = (LineMesh)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update"))
        {
            lm.UpdateLineMesh();
        }
    }
}
#endif