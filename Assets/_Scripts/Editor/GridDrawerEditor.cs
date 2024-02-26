using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GridDrawer))]
public class GridDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridDrawer gd = (GridDrawer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update"))
        {
            gd.UpdateGridMesh();
        }
    }
}
#endif