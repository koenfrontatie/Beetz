using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Metronome))]
public class EditorMetronome : Editor
{
    public override void OnInspectorGUI()
    {
        Metronome metronome = (Metronome)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Play/Pause"))
        {
            Metronome.OnTogglePlayPause?.Invoke();
        }

        if (GUILayout.Button("Reset"))
        {
            Metronome.OnResetMetronome?.Invoke();
        }
    }
}
