using System.Security.Cryptography;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public int CurrentStep { get; private set; }
    public int StepAmount { get; private set; }
    public int CurrentBeat { get; private set; }
    public int CurrentBar { get; private set; }
    public DisplayType DisplayType { get; private set; }
    public Vector3 Position { get; private set; }

    public void Init(Vector3 position, int stepAmount, DisplayType type) {
    
        this.Position = position;
        this.StepAmount = stepAmount;
        this.DisplayType = type;
    }

    private void OnEnable()
    {
        Metronome.OnBeat += CalculateBeatPosition;
        Metronome.OnBeat += CalculateBarPosition;
        Metronome.OnStep += CalculateStepPosition;
        Metronome.OnResetMetronome += ResetSequencerPlayback;
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
