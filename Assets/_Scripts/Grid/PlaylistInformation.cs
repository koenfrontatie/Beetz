using System;

using UnityEngine;

public class PlaylistInformation : MonoBehaviour
{
    public static PlaylistInformation Instance;

    [SerializeField] PlaylistData _playlistData;
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
        for(int i = 0; i < _playlistData.PositionIDData.Count; i++)
        {
            if(sequencer.SequencerData.ID == _playlistData.PositionIDData[i].ID)
            {
                var moved = new PositionID(sequencer.SequencerData.ID, _playlistData.PositionIDData[i].Position + vector);
                _playlistData.PositionIDData[i] = moved;
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
        _playlistData = new PlaylistData(DataLoader.Instance.NewGuid(), null, null);
    }

    //void LoadPlayListInfo(string s)
    //{
    //    _playlistData = DataHelper.Instance.CreateNewPlaylistInfo();
    //}

    //public void Add(Sequencer sequencer)
    //{
    //    var cell = sequencer.InstanceCellPosition;

    //    var sp = new PositionIDPair
    //    {
    //        ID = sequencer.SequencerData.ID,
    //        Position = cell
    //    };

    //    var sc = new V2Pair
    //    {
    //        one = cell,
    //        two = new Vector2(cell.x + sequencer.StepAmount - 1, cell.y + sequencer.RowAmount - 1)
    //    };
          
    //    _playlistData.PositionIDPairs.Add(sp);
    //    _playlistData.SequencerCorners.Add(sc);
    //    //Debug.Log(sp);
    //}
}
