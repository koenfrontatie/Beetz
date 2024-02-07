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
        Metronome.OnStep += CalculateStepPosition;
        Metronome.OnResetMetronome += ResetSequencerPlayback;
    }
    private void OnDisable()
    {
        Metronome.OnBeat -= CalculateBeatPosition;
        Metronome.OnStep -= CalculateStepPosition;
        Metronome.OnResetMetronome -= ResetSequencerPlayback;
    }
    
    void Start()
    {
        switch(DisplayType)
        {
            case DisplayType.Linear:
                gameObject.AddComponent<GridDisplay>(); 
                break;
            case DisplayType.Circular:
                gameObject.AddComponent<CircularDisplay>();
                break;
        }

        SequencerManager.Instance.ActiveSequencers.Add(this);
        ResetSequencerPlayback();
    }

    void CalculateBeatPosition()
    {
        CurrentBeat = (CurrentBeat % Metronome.Instance.BeatsPerBar) + 1;
        if ((CurrentBeat - 1) % Metronome.Instance.BeatsPerBar == 0) CurrentBar++;
        if (CurrentBar > StepAmount / Metronome.Instance.BeatsPerBar) CurrentBar = 1;
    }

    void CalculateStepPosition()
    {
        if (StepAmount == 0) return;
        CurrentStep = (CurrentStep % StepAmount) + 1;
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

    public void SetSteps(int amt)
    {
        StepAmount = amt;
    }
}
