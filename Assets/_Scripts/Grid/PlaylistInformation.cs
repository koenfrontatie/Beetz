using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistInformation : MonoBehaviour
{
    [SerializeField] private PlaylistInfo _playlistInfo;
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
