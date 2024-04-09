using System.Collections.Generic;
using UnityEngine;

public class ScarecrowManager : MonoBehaviour
{
    public static ScarecrowManager Instance;
    public List<Sequencer> ActiveScarecrows = new List<Sequencer>();

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
    }

    public void AddScarecrow(Sequencer sequencer)
    {
        ActiveScarecrows.Add(sequencer);
        UpdateSongRange();
    }

    void UpdateSongRange()
    {
        SongRange[0] = 1;

        for (int i = 0;i < ActiveScarecrows.Count; i++)
        {
            if (ActiveScarecrows[i].SequencerData.Dimensions.x > SongRange[1])
            {
                SongRange[1] = (int)ActiveScarecrows[i].SequencerData.Dimensions.x;
            }
        }
    }

    void UpdatePos ()
    {

    }
}
