using UnityEngine;
using UnityEngine.Events;
public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }

    private double lastBeatTime;
    private double nextBeatTime;
    private double timeSinceLastPause;
    private float lastBPM;
    private bool lastHalftime;
    private float stepProgression;
    private float lastStepProgression;
    private float beatInterval;


    public float BPM = 100f;
    public bool Halftime;
    public bool ToggleHalfTime;
    public float BeatProgression { get; private set; }
    public bool Playing { get; private set; }

    // time division
    public int BeatsPerBar;
    public int StepsPerBeat;
   
    public static UnityAction OnBeat;
    public static UnityAction OnStep;

    public static UnityAction OnResetMetronome;
    public static UnityAction<bool> OnPlayPause;
    public static UnityAction OnTogglePlayPause;
    public static UnityAction OnTempoChange;

    void Awake()
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
        ResetMetronome();
    }

    void Update()
    {
        if (Playing)
        {
            if (AudioSettings.dspTime >= nextBeatTime) 
            {
                // nextBeatTime is reached
                OnBeat?.Invoke();
                lastBeatTime = AudioSettings.dspTime;
                nextBeatTime = lastBeatTime + beatInterval;
            }

            // calculate beat progress
            BeatProgression = Mathf.Clamp01((float)(AudioSettings.dspTime - lastBeatTime) / beatInterval);
            
            // calculate step progress
            stepProgression = BeatProgression * StepsPerBeat % 1f;

            if (lastStepProgression > stepProgression) OnStep?.Invoke();
            lastStepProgression = stepProgression;
        }

        if (ToggleHalfTime)
        {
            Halftime = !Halftime;
            ToggleHalfTime = false;
        }

        TempoCheck();
    }

    public void ResetMetronome()
    {
        BeatProgression = 0;
        stepProgression = 0;
        lastBeatTime = AudioSettings.dspTime;
        beatInterval = Halftime ? 120f / BPM : 60f / BPM;

        OnResetMetronome?.Invoke();
    }

    public void PlayPauseMetronome()
    {
        if (Playing == false)
        {

            lastBeatTime = AudioSettings.dspTime - timeSinceLastPause;
            nextBeatTime = lastBeatTime + beatInterval;
            Playing = true;
        }

        else
        {
            timeSinceLastPause = AudioSettings.dspTime - lastBeatTime;
            Playing = false;
        }

        OnTogglePlayPause?.Invoke();
    }

    public void SetPlayPauseMetronome(bool b)
    {
        if (b)
        {

            lastBeatTime = AudioSettings.dspTime - timeSinceLastPause;
            nextBeatTime = lastBeatTime + beatInterval;
            Playing = true;
        }

        else
        {
            timeSinceLastPause = AudioSettings.dspTime - lastBeatTime;
            Playing = false;
        }
    }

    void TempoCheck()
    {
        if (BPM != lastBPM || Halftime != lastHalftime)
        {
            OnTempoChange?.Invoke();
        }

        lastBPM = BPM;
        lastHalftime = Halftime;
    }
}

