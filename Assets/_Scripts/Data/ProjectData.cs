using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor.SearchService;

public class ProjectData : MonoBehaviour
{
    public ProjectInfo ProjectInfo; // data struct
    
    public static ProjectData Instance;

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

    void Start()
    {
        SaveLoader.Instance.LoadProjectInfoAsync("1", Events.OnProjectInfoLoaded);
        //SaveLoader.Instance.SerializeProjectInfo();
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

        ProjectInfo.PlaylistInfo.PositionIDPairs.Add(sp);
        ProjectInfo.PlaylistInfo.SequencerCorners.Add(sc);
        
    }
    private void OnEnable()
    {
        Events.OnProjectInfoLoaded += OnProjectLoaded;
    }

    private void OnDisable()
    {
        Events.OnProjectInfoLoaded -= OnProjectLoaded;
    }

    private void OnProjectLoaded(ProjectInfo info)
    {
        var copy = new ProjectInfo
        {
            Name = info.Name,
            ID = info.ID,
            PlaylistInfo = info.PlaylistInfo,
            ToolbarConfig = info.ToolbarConfig
        };

        ProjectInfo = copy;

        Events.OnToolbarLoaded?.Invoke();
    }
}
