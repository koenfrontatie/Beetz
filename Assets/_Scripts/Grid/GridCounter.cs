using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCounter : MonoBehaviour
{
    public int[] SongRange = new int[2];
    public int CurrentStep;
    void Start()
    {
        SongRange[0] = 10000;

        SongRange[1] = -10000;
        Events.OnNewSequencer += UpdateRange;
        Events.OnRemoveSequencer += OnSequencerRemoved;
        Metronome.OnStep += UpdatePos;
        Metronome.OnResetMetronome += () => CurrentStep = SongRange[0];
    }

    private void OnDisable()
    {
        Events.OnNewSequencer -= UpdateRange;
        Events.OnRemoveSequencer -= OnSequencerRemoved;
        Metronome.OnStep -= UpdatePos;
        Metronome.OnResetMetronome -= () => CurrentStep = SongRange[0];
    }

    void UpdateRange(Vector2 cell, Vector2 dim)
    {
        var reach = (int)(cell.x + dim.x - 1);
        if (cell.x < SongRange[0])  SongRange[0] = (int)cell.x;
        if (cell.x > SongRange[1] || reach > SongRange[1])  SongRange[1] = reach;

        if(CurrentStep > SongRange[0]) CurrentStep = SongRange[0];
    }

    void OnSequencerRemoved()
    {
        SongRange[0] = 10000; // this can be improved
        SongRange[1] = -10000;
        foreach (var item in SequencerManager.Instance.ActiveSequencers) {
            UpdateRange(item.InstanceCellPosition, item.SequencerInfo.Dimensions);
        }
    }
    void UpdatePos()
    {
        CurrentStep++;

        if(CurrentStep < SongRange[0] || CurrentStep > SongRange[1])
        {
            CurrentStep = SongRange[0];
        }

    }
}
