using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EyePlacer))]
public class EditorEyePlacer : Editor
{

    public override void OnInspectorGUI()
    {

        EyePlacer placer = (EyePlacer)target;

        if (GUILayout.Button("Place"))
        {
            placer.PositionEyes();
        }


        // Draw the default inspector after your custom GUI
        DrawDefaultInspector();
    }

}
