
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
    public DisplayType DisplayType { get; private set; }
    public Vector3 InstancePosition { get; private set; }

    public Vector2 InstanceCellPosition;

    [SerializeField] private Transform _stepParent;

    /// <summary>
    /// Initialize
    /// a sequencer via sequencer data.
    /// </summary>
    /// <param name="position">World position of sequencer instance</param>
    /// <param name="data">Serialized data</param>
    /// <param name="type">Display type</param>
    public void Init(Vector3 position, SequencerData data)
    {
        SequencerData = data;
        this.DisplayType = DisplayType.Linear;

        this.StepAmount = (int)SequencerData.Dimensions.x;
        this.RowAmount = (int)SequencerData.Dimensions.y;
        
        this.InstancePosition = position;
        this.InstanceCellPosition = GridController.Instance.CellFromWorld(position);

        switch (DisplayType)
        {
            case DisplayType.Linear:
                gameObject.AddComponent<LinearDisplay>();
                break;
            case DisplayType.Circular:
                gameObject.AddComponent<CircularDisplay>();
                break;
        }

        SequencerManager.Instance.ActiveSequencers.Add(this);

        Events.SequencerBuilt?.Invoke();
    }

    private void OnEnable()
    {
        Metronome.OnBeat += CalculateBeatPosition;
        Metronome.OnBeat += CalculateBarPosition;
        Metronome.OnStep += CalculateStepPosition;
        Metronome.OnResetMetronome += ResetSequencerPlayback;
        Events.OnNewSongRange += CalculateStepPosition;
        Events.OnSequencerTapped += SequencerTappedHandler;
        Events.OnStepsPlaced += OnStepsPlaced;
        Events.OnSequencerMoved += OnMove;
    }
    private void OnDisable()
    {
        Metronome.OnBeat -= CalculateBeatPosition;
        Metronome.OnBeat -= CalculateBarPosition;
        Metronome.OnStep -= CalculateStepPosition;
        Metronome.OnResetMetronome -= ResetSequencerPlayback;
        Events.OnNewSongRange -= CalculateStepPosition;
        Events.OnSequencerTapped -= SequencerTappedHandler;
        Events.OnStepsPlaced -= OnStepsPlaced;
        Events.OnSequencerMoved -= OnMove;
    }

    void OnStepsPlaced(Sequencer s)
    {
        if (s.gameObject != transform.gameObject) return;
        InitSamplesFromInfo(SequencerData);
    }

    void OnMove(Sequencer s, Vector2 delta)
    {
        if (s.gameObject != transform.gameObject) return;
        InstanceCellPosition += delta;
        InstancePosition = transform.position;
        Events.OnUpdateGridRange?.Invoke();
    }
    private void SequencerTappedHandler(Sequencer sequencer, int stepIndex) // i think i should move this into interaction class
    {
        if (sequencer.gameObject != transform.gameObject) return;

        var selectedSample = SampleManager.Instance.SelectedSample;

        if (SampleManager.Instance.SelectedSample == null) return;

        if (_stepParent.GetChild(stepIndex).TryGetComponent<Step>(out Step selectedStep))
        {
            if (selectedStep.GetSampleObject() != null)
            {

                // remove if there is a sampleobject present
                foreach (PositionID pair in SequencerData.PositionIDData)
                {
                    if (pair.Position == GetPositionFromSiblingIndex(stepIndex))
                    {
                        //Samples.RemoveAt(Samples.IndexOf(pair));
                        SequencerData.PositionIDData.RemoveAt(SequencerData.PositionIDData.IndexOf(pair));
                        selectedStep.UnAssignSample();

                        break;
                    }
                }
                return;
            }

            var sample = Instantiate(selectedSample, selectedStep.transform);

            selectedStep.AssignSample(sample);

            string idCopy = selectedSample.SampleData.ID;
            
            if (string.IsNullOrEmpty(selectedSample.SampleData.ID))
            {
                idCopy = selectedSample.SampleData.Template.ToString();
            }

            var posID = new PositionID(idCopy, GetPositionFromSiblingIndex(stepIndex));
            
            SequencerData.PositionIDData.Add(posID);
        }
    }

    public void InitSamplesFromInfo(SequencerData sequencerData)
    {
        if (sequencerData.PositionIDData.Count == 0) return;

        for (int i = 0; i < sequencerData.PositionIDData.Count; i++)
        {
            var stepIndex = GetStepIndexFromPosition(sequencerData.PositionIDData[i].Position);

            if (_stepParent.GetChild(stepIndex).TryGetComponent<Step>(out Step selectedStep))
            {
                var spawnedSampleObject = Prefabs.Instance.BaseObjects[int.Parse(sequencerData.PositionIDData[i].ID)];
                
                SampleObject so = Instantiate(spawnedSampleObject, selectedStep.transform);

                selectedStep.AssignSample(so); // TODO - load proper sampleobject
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
    
    private void Start()
    {
        ResetSequencerPlayback();

        //StartCoroutine(TestInit());
    }

    #region position calculation
    void CalculateStepPosition()
    {
        if (StepAmount == 0) return;
        CurrentStep = (CurrentStep % StepAmount) + 1;
    }

    void CalculateBeatPosition()
    {
        CurrentBeat = (CurrentBeat % Metronome.Instance.BeatsPerBar) + 1;
    }

    void CalculateBarPosition()
    {
        var stepsPerBar = Metronome.Instance.StepsPerBeat * Metronome.Instance.BeatsPerBar;
        var barsPerLoop = StepAmount / stepsPerBar;
        CurrentBar = (int)(Mathf.Floor(CurrentStep / stepsPerBar) % (barsPerLoop)) + 1;
    }
    #endregion

    void ResetSequencerPlayback()
    {
        CurrentStep = 1;
        CurrentBeat = 1;
        CurrentBar = 1;
    }

    void OnDestroy()
    {
        SequencerManager.Instance.ActiveSequencers.Remove(this);
    }
}
