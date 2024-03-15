using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistInformation : MonoBehaviour
{
    [SerializeField] private PlaylistInfo _playlistInfo;
    private void OnEnable()
    {
        Events.OnSequencerMoved += UpdateSequencerPosition;
    }
    private void OnDisable()
    {
        Events.OnSequencerMoved -= UpdateSequencerPosition;
    }
    private void UpdateSequencerPosition(Sequencer sequencer, Vector2 vector)
    {
        for(int i = 0; i < _playlistInfo.SequencerPositions.Count; i++)
        {
            if(sequencer.SequencerInfo.ID == _playlistInfo.SequencerPositions[i].ID)
            {
                PositionIDPair moved; //= new PositionIDPair(sequencer.SequencerInfo.ID, vector);
                moved.ID = sequencer.SequencerInfo.ID;
                moved.Position = _playlistInfo.SequencerPositions[i].Position + vector;
                _playlistInfo.SequencerPositions[i] = moved;
                sequencer.InstanceCellPosition = moved.Position;
            }
        }
    }


    void Start()
    {
        LoadPlayListInfo();
    }

    void LoadPlayListInfo()
    {
        _playlistInfo = DataManager.Instance.CreateNewPlaylistInfo();
    }

    void LoadPlayListInfo(string s)
    {
        _playlistInfo = DataManager.Instance.CreateNewPlaylistInfo();
    }

    public void Add(Sequencer sequencer)
    {
        var cell = sequencer.InstanceCellPosition;

        var sp = new PositionIDPair
        {
            ID = sequencer.SequencerInfo.ID,
            Position = cell
        };

        var sc = new V2Pair
        {
            one = cell,
            two = new Vector2(cell.x + sequencer.StepAmount - 1, cell.y + sequencer.RowAmount - 1)
        };
          
        _playlistInfo.SequencerPositions.Add(sp);
        _playlistInfo.SequencerCorners.Add(sc);
        //Debug.Log(sp);
    }
}
