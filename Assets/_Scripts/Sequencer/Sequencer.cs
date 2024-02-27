using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public SequencerInfo SequencerInfo;
    //public List<Tuple<Vector2, string>> Samples;
    public List<PositionSamplePair> Samples;
    public int CurrentStep { get; private set; }
    public int StepAmount { get; private set; }
    public int RowAmount { get; private set; }
    public int CurrentBeat { get; private set; }
    public int CurrentBar { get; private set; }
    public DisplayType DisplayType { get; private set; }
    public Vector3 InstancePosition { get; private set; }



    /// <summary>
    /// Initialize
    /// an empty sequencer.
    /// </summary>
    /// <param name="position">Grid position of sequencer instance</param>
    /// <param name="info">Serialized data</param>
    public void Init(Vector3 position, DisplayType type) {
    
        SequencerInfo = DataManager.Instance.CreateNewSequencerInfo();
        SequencerInfo.Type = type;

        this.StepAmount = (int)SequencerInfo.Dimensions.x;
        this.DisplayType = SequencerInfo.Type;
        this.InstancePosition = position;
        //this.Samples = SequencerInfo.Samples;
        this.RowAmount = 1;
    }

    /// <summary>
    /// Initialize
    /// a sequencer via saved data.
    /// </summary>
    /// <param name="position">Grid position of sequencer instance</param>
    /// <param name="info">Serialized data</param>
    public void Init(Vector3 position, SequencerInfo info)
    {
        SequencerInfo = info;

        this.StepAmount = (int)SequencerInfo.Dimensions.x;
        this.DisplayType = SequencerInfo.Type;
        this.InstancePosition = position;
        //this.Samples = SequencerInfo.Samples;
    }

    private void OnEnable()
    {
        Metronome.OnBeat += CalculateBeatPosition;
        Metronome.OnBeat += CalculateBarPosition;
        Metronome.OnStep += CalculateStepPosition;
        Metronome.OnResetMetronome += ResetSequencerPlayback;

        Events.OnSequencerClicked += SequencerClickedHandler;
    }

    private void SequencerClickedHandler(Sequencer sequencer, int step)
    {
        if (sequencer != this) return;
       
        var selectedSample = SampleManager.Instance.SelectedSample;
        
        if (SampleManager.Instance.SelectedSample == null) return;

        if(transform.GetChild(step - 1).TryGetComponent<Step>(out Step selectedStep))
        {
            //if(selectedStep.transform.childCount > 0)
            //{
            //    selectedStep.transform.DestroyChildren();
            //    Destroy(selectedStep.GetComponent<Step>());
            //    selectedStep.gameObject.AddComponent<Step>();
            //} else
            //{

            var sample = Instantiate(selectedSample, selectedStep.transform);
            
            selectedStep.AssignSample(sample);
            // TODO save this data
        
            var positionData = new PositionSamplePair();
            positionData.ID = selectedSample.Info.ID;
            
            if (string.IsNullOrEmpty(selectedSample.Info.ID))
            {
                positionData.ID = selectedSample.Info.Template.ToString();
            }

            positionData.Position = new Vector2(1, step); 
            Samples.Add(positionData);   
        }
    }

    private void OnDisable()
    {
        Metronome.OnBeat -= CalculateBeatPosition;
        Metronome.OnBeat -= CalculateBarPosition;
        Metronome.OnStep -= CalculateStepPosition;
        Metronome.OnResetMetronome -= ResetSequencerPlayback;
    }
    
    void Start()
    {
        switch(DisplayType)
        {
            case DisplayType.Linear:
                gameObject.AddComponent<LinearDisplay>(); 
                break;
            case DisplayType.Circular:
                gameObject.AddComponent<CircularDisplay>();
                break;
        }

        SequencerManager.Instance.ActiveSequencers.Add(this);
        ResetSequencerPlayback();
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
