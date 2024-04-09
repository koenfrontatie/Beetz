using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LinearListener : MonoBehaviour, IListener
{
    private Sequencer _sequencer;
    void Awake()
    {
        _sequencer = GetComponent<Sequencer>();
    }

    public int GetStepPosition()
    {
        if (!IsGlobalPlaybackInRange()) return -1;

        return 1 + LinearPlayback.Instance.CurrentPosition - (int)_sequencer.InstanceCellPosition.x;
    }

    public int GetBeatPosition()
    {
        if (_sequencer.CurrentStep == -1) return -1;

        if (_sequencer.CurrentStep == 1)
        {
            return 1;
        }

        if ((_sequencer.CurrentStep - 1) % Metronome.Instance.StepsPerBeat == 0)
        {
            return (_sequencer.CurrentBeat % Metronome.Instance.BeatsPerBar) + 1;
        }

        return _sequencer.CurrentBeat;
    }

    public int GetBarPosition()
    {
        if (_sequencer.CurrentStep == -1) return -1;

        if (_sequencer.CurrentStep == 1)
        {
            return 1;
        }

        return 1 + Mathf.CeilToInt((_sequencer.CurrentStep - 1) / (Metronome.Instance.StepsPerBeat * Metronome.Instance.BeatsPerBar));
    }

    bool IsGlobalPlaybackInRange()
    {
        if (LinearPlayback.Instance.CurrentPosition >= _sequencer.InstanceCellPosition.x && LinearPlayback.Instance.CurrentPosition < _sequencer.InstanceCellPosition.x + _sequencer.StepAmount)
        {
            return true;
        }
        
        return false;
    }

    private void Start()
    {
        LinearPlayback.Instance.AddSequencer(_sequencer);
    }

    private void OnDestroy()
    {
        LinearPlayback.Instance.RemoveSequencer(_sequencer);
    }
}

