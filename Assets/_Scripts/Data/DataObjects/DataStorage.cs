using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DataStorage : MonoBehaviour
{
    public static DataStorage Instance;

    [Header("Current Project")]
    public ProjectData ProjectData;

    public string ProjectPath;

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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S)) /*SaveLoader.Instance.SerializeProjectData();*/ SaveLoader.Instance.SaveData(int.Parse(AssetBuilder.Instance.SelectedGuid));
        //if (Input.GetKeyDown(KeyCode.L)) SaveLoader.Instance.LoadProjectInfoAsync("1", Events.ProjectDataLoaded);
    }

    public async void OpenLastProject()
    {
        var dirInfo = new DirectoryInfo(Utils.ProjectSavepath);
        var projectDirectories = dirInfo.GetDirectories().OrderByDescending(d => d.LastWriteTime).ToList();

        if (projectDirectories.Count > 0) // if there are projects available, open the most recent
        {
            ProjectPath = projectDirectories[0].FullName;
            var projectDataPath = Path.Combine(ProjectPath, "ProjectData.json");
            //SaveLoader.Instance.DeserializeProjectData(projectDataPath);
            var projectData = await SaveLoader.Instance.DeserializeProjectData(projectDataPath);
            OnProjectDataLoaded(projectData);

            //Events.OpenNewProject?.Invoke();
        }
        else // if there are no projects available, open a new project
        {
            await OpenNewProject();
        }

        Events.ProjectDataLoaded?.Invoke(ProjectData);
    }

    public async void OpenNewProjectVoid() {
        await OpenNewProject();
    }
    
    public async Task OpenNewProject()
    {
        var newGUID = SaveLoader.Instance.NewGuid();
        ProjectPath = Path.Combine(Utils.ProjectSavepath, newGUID);
        var newSampleDirectory = Path.Combine(Utils.SampleSavepath, newGUID);
        Utils.CheckForCreateDirectory(ProjectPath);
        Utils.CheckForCreateDirectory(newSampleDirectory);
        var projectData = await SaveLoader.Instance.DeserializeProjectData(Path.Combine(Application.streamingAssetsPath, "ProjectData.json"));
        projectData.ID = newGUID;
        SaveLoader.Instance.SaveData(Path.Combine(ProjectPath, "ProjectData.json"), projectData);
        OnProjectDataLoaded(projectData);

        Events.ProjectDataLoaded?.Invoke(ProjectData);
    }

    public void OnProjectDataLoaded(ProjectData data)
    {
        var newdata = new ProjectData(data.ID, data.PlaylistData, data.ToolbarConfiguration, data.SampleCollection, data.SequencerDataCollection);
        ProjectData = newdata;
        GameManager.Instance.UpdateState(GameState.Gameplay);
        Events.LoadingToolbar?.Invoke(data.ToolbarConfiguration.IDC);
        AssetBuilder.Instance.SearchForCustomSamples();
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
}

//----------------------------------------------- Data storage classes

[System.Serializable]
public class SampleData
{
    public string ID;
    public string Name;
    public int Template;

    public List<EffectType> Effects;
    public List<float> EffectValues;

    public SampleData(string id, string name, int template, List<EffectType> effects, List<float> effectValues)
    {
        ID = id;
        Name = name;
        Template = template;
        Effects = effects;
        EffectValues = effectValues;
    }
}

public enum EffectType
{
    Reverb,
    Distortion,
    Delay,
    Chorus
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
    public IDCollection ToolbarConfiguration;
    public IDCollection SampleCollection;
    public List<SequencerData> SequencerDataCollection;

    public ProjectData(string id, PlaylistData pdata, IDCollection toolbar, IDCollection idcol, List<SequencerData> sequencerData)
    {
        ID = id;
        PlaylistData = pdata;
        ToolbarConfiguration = toolbar;
        SampleCollection = idcol;
        SequencerDataCollection = sequencerData;
    }
}