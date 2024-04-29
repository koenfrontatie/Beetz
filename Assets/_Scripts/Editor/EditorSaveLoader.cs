using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SaveLoader))]
public class EditorSaveLoader : Editor
{
    public override void OnInspectorGUI()
    {
        SaveLoader saveloader = (SaveLoader)target;



        if (GUILayout.Button("Load"))
        {
            saveloader.LoadFromEditor();
        }

        DrawDefaultInspector();
        //if (GUILayout.Button("Settings"))
        //{
        //    controller.OnStateChanged(GameState.Menu);
        //}

        //if (GUILayout.Button("Library"))
        //{
        //    controller.OnStateChanged(GameState.Library);
        //}
    }
}
#endif