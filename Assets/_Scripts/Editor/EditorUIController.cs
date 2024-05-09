using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(UIController))]
public class EditorUIController : Editor
{
    public override void OnInspectorGUI()
    {
        UIController controller = (UIController)target;

        DrawDefaultInspector();

        if (GUILayout.Button("MainScene"))
        {
            controller.OnStateChanged(GameState.Gameplay);
        }

        if (GUILayout.Button("Settings"))
        {
            controller.OnStateChanged(GameState.Menu);
        }
        
        if (GUILayout.Button("Library"))
        {
            controller.OnStateChanged(GameState.Library);
        }

        if (GUILayout.Button("Biolab"))
        {
            controller.OnStateChanged(GameState.Biolab);
        }
    }
}
#endif