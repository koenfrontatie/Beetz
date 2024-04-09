using System.Collections.Generic;
using UnityEngine;
public class LinearPlayback : MonoBehaviour
{
    public static LinearPlayback Instance;

    [SerializeField] private PlaybackController _controller;

    public List<Sequencer> LinearSequencers = new List<Sequencer>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public int[] SongRange = new int[2];
    public int CurrentPosition;
    private void OnEnable()
    {
        Events.UpdateLinearRange += UpdateRange;
        Metronome.NewStep += UpdatePos;
        Metronome.ResetMetronome += () => CurrentPosition = SongRange[0];
    }
    void Start()
    {
        SongRange[0] = 10000;

        SongRange[1] = -10000;
    }

    private void OnDisable()
    {
        Events.UpdateLinearRange -= UpdateRange;
        Metronome.NewStep -= UpdatePos;
        Metronome.ResetMetronome -= () => CurrentPosition = SongRange[0];
    }

    void UpdateRange()
    {
        var lastMin = SongRange[0];
        var lastMax = SongRange[1];

        SongRange[0] = 10000;
        SongRange[1] = -10000;

        bool newRange = false;

        for (int i = 0; i < LinearSequencers.Count; i++)
        {
            var seq = LinearSequencers[i];
            var cell = seq.InstanceCellPosition;
            var dim = seq.SequencerData.Dimensions;
            var reach = (int)(cell.x + dim.x - 1);

            if (cell.x < SongRange[0])
            {
                SongRange[0] = (int)cell.x;
                newRange = true;
            }

            if (reach > SongRange[1])
            {
                SongRange[1] = reach;
                newRange = true;
            }

            //if (newRange)
            //{
            //    var mindelta = lastMin - SongRange[0];
            //    CurrentPosition += mindelta;

            //    var maxDelta = lastMax - SongRange[1];
            //    CurrentPosition -= maxDelta;

            //    Events.OnNewSongRange?.Invoke();
            //}

            if (CurrentPosition > SongRange[1]) CurrentPosition = SongRange[0];

            // TODO - recalculate currentstep position so playback works while moving seq
        }
    }
    void UpdatePos()
    {
        if (_controller.PlaybackMode != PlaybackMode.Linear) return;

        CurrentPosition++;

        if (CurrentPosition < SongRange[0] || CurrentPosition > SongRange[1])
        {
            CurrentPosition = SongRange[0];
        }
    }

    public void AddSequencer(Sequencer sequencer)
    {
        LinearSequencers.Add(sequencer);
        UpdateRange();
        sequencer.UpdateStepPosition();
    }

    public void RemoveSequencer(Sequencer sequencer)
    {
        LinearSequencers.Remove(sequencer);
        UpdateRange();
    }
}
