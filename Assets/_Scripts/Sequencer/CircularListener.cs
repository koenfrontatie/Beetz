using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularListener : MonoBehaviour, IListener
{
    private Sequencer _sequencer;
    void Awake()
    {
        _sequencer = GetComponent<Sequencer>();
    }
    public int GetStepPosition()
    {
        if (!IsGlobalPlaybackInRange() || PlaybackController.Instance.PlaybackMode == PlaybackMode.Linear) return -1;

        return CircularPlayback.Instance.CurrentPosition;
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
        if (CircularPlayback.Instance.CurrentPosition >= 1 && CircularPlayback.Instance.CurrentPosition <= _sequencer.StepAmount)
        {
            return true;
        }

        return false;
    }

    private void Start()
    {
        CircularPlayback.Instance.CircularSequencers.Add(_sequencer);
    }

    private void OnDestroy()
    {
        CircularPlayback.Instance.CircularSequencers.Remove(_sequencer);
    }
}

