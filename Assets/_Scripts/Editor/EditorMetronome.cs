using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Metronome))]
public class EditorMetronome : Editor
{
    public override void OnInspectorGUI()
    {
        Metronome metronome = (Metronome)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Play/Pause"))
        {
            metronome.PlayPauseMetronome();
        }

        if (GUILayout.Button("Reset"))
        {
            metronome.ResetMetronome();
        }
    }
}
#endif