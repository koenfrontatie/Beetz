using JetBrains.Annotations;
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
        
        Events.OnNewSequencerBuilt += UpdateRange;
        Events.OnRemoveSequencer += UpdateRange;
        Events.OnSequencerMoved += (Sequencer seq, Vector2 offset) => UpdateRange();
        Metronome.OnStep += UpdatePos;
        Metronome.OnResetMetronome += () => CurrentStep = SongRange[0];
    }

    private void OnDisable()
    {
        Events.OnNewSequencerBuilt -= UpdateRange;
        Events.OnRemoveSequencer -= UpdateRange;
        Events.OnSequencerMoved -= (Sequencer seq, Vector2 offset) => UpdateRange();
        Metronome.OnResetMetronome -= () => CurrentStep = SongRange[0];
    }

    //void UpdateRange(Vector2 cell, Vector2 dim)
    //{   //needs to loop over all active sequencers
    //    var reach = (int)(cell.x + dim.x - 1);
    //    if (cell.x < SongRange[0]) SongRange[0] = (int)cell.x;
    //    if (cell.x > SongRange[1] || reach > SongRange[1]) SongRange[1] = reach;

    //    if (CurrentStep > SongRange[0]) CurrentStep = SongRange[0];
    //}

    void UpdateRange()
    {
        var lastMin = SongRange[0];
        var lastMax = SongRange[1];
        
        SongRange[0] = 10000;
        SongRange[1] = -10000;

        bool newRange = false;

        for (int i = 0; i < SequencerManager.Instance.ActiveSequencers.Count; i++)
        {
            var seq = SequencerManager.Instance.ActiveSequencers[i];
            var cell = seq.InstanceCellPosition;
            var dim = seq.SequencerInfo.Dimensions;
            var reach = (int)(cell.x + dim.x - 1);
            
            if (cell.x < SongRange[0])
            {
                SongRange[0] = (int)cell.x;
                newRange = true;
            }
            
            if (cell.x > SongRange[1] || reach > SongRange[1])
            {
                SongRange[1] = reach;
                newRange = true;
            }

            if(newRange)
            {
                var mindelta = lastMin - SongRange[0];
                CurrentStep += mindelta;

                var maxDelta = lastMax - SongRange[1];
                CurrentStep -= maxDelta;

                Events.OnNewSongRange?.Invoke();
            }

            if (CurrentStep > SongRange[0]) CurrentStep = SongRange[0];

            // TODO - recalculate currentstep position so playback works while moving seq
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
