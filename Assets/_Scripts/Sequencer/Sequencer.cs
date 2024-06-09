using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Sequencer : MonoBehaviour
{
    public SequencerData SequencerData;
    public int CurrentStep { get; private set; }
    public int StepAmount { get; private set; }
    public int RowAmount { get; private set; }
    public int CurrentBeat { get; private set; }
    public int CurrentBar { get; private set; }
    public IDisplayer Displayer { get; private set; }
    public IListener PlaybackListener { get; private set; }
    public Vector3 InstancePosition { get; private set; }

    public Vector2 InstanceCellPosition;

    [SerializeField] private Transform _stepParent;
    private void OnEnable()
    {
        Metronome.NewStep += UpdateStepPosition;
        Metronome.ResetMetronome += UpdateStepPosition;
        Events.OnStepsPlaced += OnStepsPlaced;
        Events.MoveSequencer += OnMove;
        Events.ResizeSequencer += OnResize;

    }

    private void OnResize(Vector2 position, SequencerData data)
    {
        if(data.ID != SequencerData.ID) return;

        
        //throw new NotImplementedException();
        //Vector2 delta = InstanceCellPosition - position
        this.StepAmount = (int)data.Dimensions.x;
        this.RowAmount = (int)data.Dimensions.y;
        this.InstancePosition = GridController.Instance.WorldFromCell(position) + new Vector3(Config.CellSize * .5f, 0f, Config.CellSize * .5f);
        this.InstanceCellPosition = position;
        //Debug.Log($"delta {delta}  instancecell {InstanceCellPosition}  position {position}");
        transform.position = new Vector3(InstancePosition.x, transform.position.y, InstancePosition.z);

        SequencerData = data;

        Displayer.SpawnSteps();

        var rockplacer = transform.GetComponentInChildren<RockPlacer>();

        if(rockplacer != null)
        {
            rockplacer.Place(InstancePosition, new Vector2(StepAmount, RowAmount));
        }
    }

    /// <summary>
    /// Initialize
    /// a sequencer via sequencer data.
    /// </summary>
    /// <param name="position">World position of sequencer instance</param>
    /// <param name="data">Serialized data</param>
    /// <param name="type">Display type</param>
    public void Init(Vector3 position, SequencerData data)
    {
        Displayer = GetComponent<IDisplayer>();
        PlaybackListener = GetComponent<IListener>();

        SequencerData = data;

        this.StepAmount = (int)SequencerData.Dimensions.x;
        this.RowAmount = (int)SequencerData.Dimensions.y;
        
        this.InstancePosition = position;
        this.InstanceCellPosition = GridController.Instance.CellFromWorld(position);

        //DataStorage.Instance.AddSequencer(this);

        Displayer.SpawnSteps();

        SequencerManager.Instance.ActiveSequencers.Add(this);

        Events.SequencerBuilt?.Invoke(this);
    }

    public void UpdateStepPosition()
    {
        if (GameManager.Instance.State != GameState.Gameplay) return;
        CurrentStep = PlaybackListener.GetStepPosition();
        CurrentBeat = PlaybackListener.GetBeatPosition();
        CurrentBar = PlaybackListener.GetBarPosition();

        Displayer.UpdateStepColors();
    }

    void OnStepsPlaced(GameObject obj)
    {
        if (obj != gameObject) return;
        InitSamplesFromInfo(SequencerData);
    }

    void OnMove(Sequencer s, Vector2 delta)
    {
        if (s.gameObject != transform.gameObject) return;
        InstanceCellPosition += delta;
        InstancePosition = transform.position;
        //DataStorage.Instance.UpdateSequencerPosition(s);
        Events.UpdateLinearRange?.Invoke();
    }

    public async void InitSamplesFromInfo(SequencerData sequencerData)
    {
        if (sequencerData.PositionIDData.Count == 0) return;

        for (int i = 0; i < sequencerData.PositionIDData.Count; i++)
        {
            var stepIndex = GetStepIndexFromPosition(sequencerData.PositionIDData[i].Position);
            
            if (_stepParent.GetChild(stepIndex).TryGetComponent<Step>(out Step selectedStep))
            {
                //var spawnedSampleObject = Prefabs.Instance.BaseObjects[int.Parse(sequencerData.PositionIDData[i].ID)];
                //var spawnedSampleObject = await AssetBuilder.Instance.GetSampleObject(sequencerData.PositionIDData[i].ID);

                //SampleObject so = Instantiate(spawnedSampleObject, selectedStep.transform);

                var instance = await AssetBuilder.Instance.GetSampleObject(sequencerData.PositionIDData[i].ID);
                //var instance = Instantiate(await AssetBuilder.Instance.GetSampleObject(selectedGuid), matchingStep.transform);
                instance.transform.SetParent(selectedStep.transform);
                instance.transform.position = selectedStep.transform.position;

                //matchingStep.AssignSample(instance);
                //Debug.Log($"this is i is {i} ,{selectedStep.BeatIndex}");
                selectedStep.AssignSample(instance); // TODO - load proper sampleobject
            }
        }
    }

    public int GetStepIndexFromPosition(Vector2 position)
    {
        var yindex = position.y - 1;
        return (int)((SequencerData.Dimensions.x * yindex) + position.x) - 1;
    }
    
    public Vector2 GetPositionFromSiblingIndex(int index)
    {
        var x = (index % SequencerData.Dimensions.x) + 1;
        var y = 1 + ((index - (x-1)) / SequencerData.Dimensions.x);
        return new Vector2(x, y);
    }

    void OnDestroy()
    {
        SequencerManager.Instance.ActiveSequencers.Remove(this);
        //DataStorage.Instance.RemoveSequencer(this);
    }

    private void OnDisable()
    {
        Metronome.NewStep -= UpdateStepPosition;
        Metronome.ResetMetronome -= UpdateStepPosition;
        Events.OnNewSongRange -= UpdateStepPosition;
        Events.OnStepsPlaced -= OnStepsPlaced;
        Events.MoveSequencer -= OnMove;
        Events.ResizeSequencer -= OnResize;
    }
}
