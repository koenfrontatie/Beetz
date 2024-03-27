using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    public static DataStorage Instance;

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
    private void OnEnable()
    {
        Events.ProjectDataLoaded += OnProjectDataLoaded;
    }

    private void OnDisable()
    {
        Events.ProjectDataLoaded -= OnProjectDataLoaded;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) SaveLoader.Instance.SerializeProjectData();
        //if (Input.GetKeyDown(KeyCode.L)) SaveLoader.Instance.LoadProjectInfoAsync("1", Events.ProjectDataLoaded);
    }

    public void OpenNewProject()
    {
        //SaveLoader.Instance.LoadProjectDataAsyncFullPath(Path.Combine(Application.streamingAssetsPath, "ProjectData.json"), Events.ProjectDataLoaded);
        SaveLoader.Instance.DeserializeProjectData(Path.Combine(Application.streamingAssetsPath, "ProjectData.json"));
    }

    public void OnProjectDataLoaded(ProjectData data)
    {
        var newdata = new ProjectData(data.ID, data.PlaylistData, data.IDCollection, data.SequencerDataCollection);
        GameManager.Instance.UpdateState(GameState.Gameplay);
        ProjectData = newdata;
        Events.LoadingToolbar?.Invoke(data.IDCollection.IDC);
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
    public List<SequencerData> SequencerDataCollection;

    public ProjectData(string id, PlaylistData pdata, IDCollection idcol, List<SequencerData> sequencerData)
    {
        ID = id;
        PlaylistData = pdata;
        IDCollection = idcol;
        SequencerDataCollection = sequencerData;
    }
}