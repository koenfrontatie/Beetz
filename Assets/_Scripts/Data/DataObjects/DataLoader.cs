using System;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance;

    public ProjectData ProjectData;

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

    public string NewGuid()
    {
        return Guid.NewGuid().ToString();
    }

    public void AddSequencer(Sequencer sequencer)
    {
        var cell = sequencer.InstanceCellPosition;

        var sp = new PositionID(sequencer.SequencerData.ID, cell);

        var sc = new Vector2Pair(cell, new Vector2(cell.x + sequencer.StepAmount - 1, cell.y + sequencer.RowAmount - 1));

        ProjectData.PlaylistData.PositionIDData.Add(sp);
        ProjectData.PlaylistData.SequencerCorners.Add(sc);

    }
}

//----------------------------------------------- Data storage classes

[System.Serializable]
public class SampleData
{
    public string ID;
    public string Name;
    public int Template;

    public SampleData(string id, string name, int template)
    {
        ID = id;
        Name = name;
        Template = template;
    }
}

[System.Serializable]
public class SequencerData
{
    public string ID;
    public Vector2 Dimensions;
    public List<PositionID> PositionIDData;

    public SequencerData(string id, Vector2 dimensions, List<PositionID> positionIDData)
    {
        ID = id;
        Dimensions = dimensions;
        PositionIDData = positionIDData;
    }
}

[System.Serializable]
public class PositionID
{
    public Vector2 Position;
    public string ID;

    public PositionID(string id, Vector2 position)
    {
        ID = id;
        Position = position;
    }
}

[System.Serializable]
public class PlaylistData
{
    public string ID;
    public List<PositionID> PositionIDData;
    public List<Vector2Pair> SequencerCorners;

    public PlaylistData(string id, List<PositionID> pidData, List<Vector2Pair> sequencerCorners)
    {
        ID = id;
        PositionIDData = pidData;
        SequencerCorners = sequencerCorners;
    }
}

[System.Serializable]
public class Vector2Pair
{
    public Vector2 One;
    public Vector2 Two;

    public Vector2Pair(Vector2 one, Vector2 two)
    {
        this.One = one;
        this.Two = two;
    }
}

[System.Serializable]
public class IDCollection
{
    public List<string> IDC;

    public IDCollection(List<string> idc)
    {
        IDC = idc;
    }
}

[System.Serializable]
public class ProjectData
{
    public string ID;
    public PlaylistData PlaylistData;
    public IDCollection IDCollection;
    public ProjectData(string id, PlaylistData pdata, IDCollection idcol)
    {
        ID = id;
        PlaylistData = pdata;
        IDCollection = idcol;
    }
}