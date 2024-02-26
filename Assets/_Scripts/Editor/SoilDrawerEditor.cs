using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

#if UNITY_EDITOR
[CustomEditor(typeof(SoilDrawer))]
public class SoilDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoilDrawer sd = (SoilDrawer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Update"))
        {
            sd.Init();
        }
    }
}
#endif


