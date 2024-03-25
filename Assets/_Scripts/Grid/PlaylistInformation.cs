using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SearchService;
using UnityEngine;

public class PlaylistInformation : MonoBehaviour
{
    public static PlaylistInformation Instance;

    [SerializeField] private PlaylistInfo _playlistInfo;
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

    private void UpdateSequencerPosition(Sequencer sequencer, Vector2 vector)
    {
        for(int i = 0; i < _playlistInfo.PositionIDPairs.Count; i++)
        {
            if(sequencer.SequencerInfo.ID == _playlistInfo.PositionIDPairs[i].ID)
            {
                PositionIDPair moved; //= new PositionIDPair(sequencer.SequencerInfo.ID, vector);
                moved.ID = sequencer.SequencerInfo.ID;
                moved.Position = _playlistInfo.PositionIDPairs[i].Position + vector;
                _playlistInfo.PositionIDPairs[i] = moved;
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
        _playlistInfo = DataHelper.Instance.CreateNewPlaylistInfo();
    }

    void LoadPlayListInfo(string s)
    {
        _playlistInfo = DataHelper.Instance.CreateNewPlaylistInfo();
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
          
        _playlistInfo.PositionIDPairs.Add(sp);
        _playlistInfo.SequencerCorners.Add(sc);
        //Debug.Log(sp);
    }
}
