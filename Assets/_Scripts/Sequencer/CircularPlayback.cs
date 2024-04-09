using System.Collections.Generic;
using UnityEngine;

public class CircularPlayback : MonoBehaviour
{
    public static CircularPlayback Instance;
    
    [SerializeField] private PlaybackController _controller;

    public List<Sequencer> CircularSequencers = new List<Sequencer>();

    public int[] SongRange = new int[2];
    public int CurrentPosition;

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

    private void OnEnable()
    {
        Metronome.NewStep += UpdatePos;
        Metronome.ResetMetronome += () => CurrentPosition = SongRange[0];
        Metronome.ResetMetronome += UpdateRange;
        Events.UpdateCircularRange += UpdateRange;

    }

    void UpdateRange()
    {
        SongRange[0] = 1;

        for (int i = 0; i < CircularSequencers.Count; i++)
        {
            if (CircularSequencers[i].SequencerData.Dimensions.x > SongRange[1])
            {
                SongRange[1] = (int)CircularSequencers[i].SequencerData.Dimensions.x;
            }
        }
    }

    void UpdatePos()
    {
        if (_controller.PlaybackMode != PlaybackMode.Circular) return;
        CurrentPosition = (CurrentPosition + 1) % (SongRange[1] + 1);
        if( CurrentPosition == 0)
        {
            CurrentPosition = 1;
        }
    }

    public void AddSequencer(Sequencer sequencer)
    {
        CircularSequencers.Add(sequencer);
        UpdateRange();
        sequencer.UpdateStepPosition();
    }

    public void RemoveSequencer(Sequencer sequencer)
    {
        CircularSequencers.Remove(sequencer);
        UpdateRange();
        sequencer.UpdateStepPosition();
    }

    private void OnDisable()
    {
        Metronome.NewStep -= UpdatePos;
        Metronome.ResetMetronome -= () => CurrentPosition = SongRange[0];
        Metronome.ResetMetronome -= UpdateRange;
        Events.UpdateCircularRange -= UpdateRange;
    }
}
