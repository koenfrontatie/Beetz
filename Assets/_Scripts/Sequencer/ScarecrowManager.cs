using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
        ActiveScarecrows.Clear(); //quick hack
        ActiveScarecrows.Add(sequencer);

        // UpdateSongRange asynchronously
        UpdateSongRangeAsync(sequencer);
    }

    private async void UpdateSongRangeAsync(Sequencer sequencer)
    {
        await Task.Run(() => UpdateSongRange(sequencer));

        // Invoke events on the main thread
        if (ActiveScarecrows.Count < 1) return;
        Events.UpdateCircularRange?.Invoke();
    }

    public void CheckRemove(Sequencer sequencer)
    {
        if (sequencer == null) return;
        if(ActiveScarecrows.Count < 1) return;
        if (sequencer.SequencerData == null) return;

        if (sequencer.SequencerData.ID == ActiveScarecrows[0].SequencerData.ID)
        {
            Destroy(ActiveScarecrows[0].gameObject);
            ActiveScarecrows.Clear();
        }
    }

    void UpdateSongRange(Sequencer sequencer)
    {
        //SongRange[0] = 1;

        //for (int i = 0; i < ActiveScarecrows.Count; i++)
        //{
        //    if (ActiveScarecrows[i].SequencerData.Dimensions.x > SongRange[1])
        //    {
        //        SongRange[1] = (int)ActiveScarecrows[i].SequencerData.Dimensions.x;
        //    }
        //}
        SongRange[1] = (int)sequencer.SequencerData.Dimensions.x;

        //quick hack that works for 1 scarecrow
        //SongRange[1] = (int)ActiveScarecrows[i].SequencerData.Dimensions.x;

    }

    void UpdatePos ()
    {

    }
}
