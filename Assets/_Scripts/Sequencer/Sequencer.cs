using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.PackageManager.UI;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public SequencerInfo SequencerInfo;
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
    /// <param name="info">Serialized data</param>
    /// <param name="type">Display type</param>
    public void Init(Vector3 position, SequencerInfo info)
    {
        SequencerInfo = info;
        this.DisplayType = info.Type;

        this.StepAmount = (int)SequencerInfo.Dimensions.x;
        this.RowAmount = (int)SequencerInfo.Dimensions.y;
        
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

        Events.OnNewSequencerBuilt?.Invoke();
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
    }

    void OnStepsPlaced(Sequencer s)
    {
        if (s.gameObject != transform.gameObject) return;
        InitSamplesFromInfo(SequencerInfo);
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
                foreach (PositionIDPair pair in SequencerInfo.PositionIDPairs)
                {
                    if (pair.Position == GetPositionFromSiblingIndex(stepIndex))
                    {
                        //Samples.RemoveAt(Samples.IndexOf(pair));
                        SequencerInfo.PositionIDPairs.RemoveAt(SequencerInfo.PositionIDPairs.IndexOf(pair));
                        selectedStep.UnAssignSample();

                        break;
                    }
                }
                return;
            }

            var sample = Instantiate(selectedSample, selectedStep.transform);

            selectedStep.AssignSample(sample);
            // TODO save this data

            var positionData = new PositionIDPair();
            positionData.ID = selectedSample.Info.ID;

            if (string.IsNullOrEmpty(selectedSample.Info.ID))
            {
                positionData.ID = selectedSample.Info.Template.ToString();
            }

            positionData.Position = GetPositionFromSiblingIndex(stepIndex);
            SequencerInfo.PositionIDPairs.Add(positionData);

        }
    }

    public void InitSamplesFromInfo(SequencerInfo sequencerInfo)
    {
        if (sequencerInfo.PositionIDPairs.Count == 0) return;

        for (int i = 0; i < sequencerInfo.PositionIDPairs.Count; i++)
        {
            var stepIndex = GetStepIndexFromPosition(sequencerInfo.PositionIDPairs[i].Position);

            if (_stepParent.GetChild(stepIndex).TryGetComponent<Step>(out Step selectedStep))
            {
                var spawnedSampleObject = Prefabs.Instance.BaseObjects[int.Parse(sequencerInfo.PositionIDPairs[i].ID)];
                
                SampleObject so = Instantiate(spawnedSampleObject, selectedStep.transform);

                selectedStep.AssignSample(so); // TODO - load proper sampleobject
            }
        }
    }

    public int GetStepIndexFromPosition(Vector2 position)
    {
        Debug.Log("position is " + position);
        var yindex = position.y - 1;
        Debug.Log("getstepindex is " + ((int)((SequencerInfo.Dimensions.x * yindex) + position.x) - 1));

        return (int)((SequencerInfo.Dimensions.x * yindex) + position.x) - 1;
    }
    
    public Vector2 GetPositionFromSiblingIndex(int index)
    {
        var x = (index % SequencerInfo.Dimensions.x) + 1;
        var y = 1 + ((index - (x-1)) / SequencerInfo.Dimensions.x);
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
