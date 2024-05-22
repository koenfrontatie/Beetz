using System.Collections.Generic;
using System;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    /// <summary>
    /// This class is responsible for storing runtime data.
    /// </summary>
    
    public static DataStorage Instance;
    
    public ProjectData ProjectData;

    [SerializeField] private KVDW.Logger _logger;

    public static Action<ProjectData> ProjectDataSet;

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

    public void SetProjectData(ProjectData data)
    {
        ProjectData = data;
        ProjectDataSet?.Invoke(ProjectData);
        //AssetBuilder.Instance.SearchForCustomSamples();
        //AssetBuilder.Instance.FindCustomTextures();
    }

    public void AddSequencer(Sequencer sequencer)
    {
        var cell = sequencer.InstanceCellPosition;

        var sp = new PositionID(sequencer.SequencerData.ID, cell);
        // corners should prob b in sequencerdata 
        var sc = new Vector2Pair(cell, new Vector2(cell.x + sequencer.StepAmount - 1, cell.y + sequencer.RowAmount - 1));

        ProjectData.PlaylistData.PositionIDData.Add(sp);
        ProjectData.PlaylistData.SequencerCorners.Add(sc);
        ProjectData.SequencerDataCollection.Add(sequencer.SequencerData);
    }

    public void UpdateSequencerPosition(Sequencer sequencer)
    {
        var list = ProjectData.PlaylistData.PositionIDData;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ID == sequencer.SequencerData.ID)
            {
                list[i].Position = sequencer.InstanceCellPosition;
                // corners should prob b in sequencerdata 
                var sc = new Vector2Pair(sequencer.InstanceCellPosition, new Vector2(sequencer.InstanceCellPosition.x + sequencer.StepAmount - 1, sequencer.InstanceCellPosition.y + sequencer.RowAmount - 1)); 

                ProjectData.PlaylistData.SequencerCorners[i] = sc;
                
                return;
            }
        }
    }

    public void RemoveSequencer(Sequencer sequencer)
    {
        var list = ProjectData.PlaylistData.PositionIDData;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ID == sequencer.SequencerData.ID)
            {
                list.RemoveAt(i);
                ProjectData.PlaylistData.SequencerCorners.RemoveAt(i);
                ProjectData.SequencerDataCollection.RemoveAt(i);
                return;
            }
        }
    }

    void Log(object message)
    {
        if(_logger)
            _logger.Log(message, this);
    }
}

//----------------------------------------------- Data storage classes

[System.Serializable]
public class SampleData
{
    public string ID;
    public string Name;
    public int Template;

    public List<EffectValuePair> Effects;

    public SampleData(string id, string name, int template, List<EffectValuePair> effects)
    {
        ID = id;
        Name = name;
        Template = template;
        Effects = effects;
    }
}

[System.Serializable]
public enum EffectType
{
    Reverb,
    Distortion,
    Delay,
    Chorus,
    Pitch
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
public class EffectValuePair
{
    public EffectType Effect;
    public float Value;

    public EffectValuePair(EffectType effect, float value)
    {
        Effect = effect;
        Value = value;
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
    public IDCollection ToolbarConfiguration;
    public IDCollection SampleCollection;
    public List<SequencerData> SequencerDataCollection;
    // todo add bpm
    public ProjectData(string id, PlaylistData pdata, IDCollection toolbar, IDCollection idcol, List<SequencerData> sequencerData)
    {
        ID = id;
        PlaylistData = pdata;
        ToolbarConfiguration = toolbar;
        SampleCollection = idcol;
        SequencerDataCollection = sequencerData;
    }
}