using UnityEngine;
using UnityEngine.Events;
public class Metronome : MonoBehaviour
{
    public static Metronome Instance { get; private set; }

    private float lastBeatTime;
    private float nextBeatTime;
    private float timeSinceLastPause;
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
            if (Time.time >= nextBeatTime) 
            {
                // nextBeatTime is reached
                OnBeat?.Invoke();
                lastBeatTime = Time.time;
                nextBeatTime = lastBeatTime + beatInterval;
            }

            // calculate beat progress
            BeatProgression = Mathf.Clamp01((Time.time - lastBeatTime) / beatInterval);
            
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
        lastBeatTime = Time.time;
        beatInterval = Halftime ? 120f / BPM : 60f / BPM;

        OnResetMetronome?.Invoke();
    }

    public void PlayPauseMetronome()
    {
        if (Playing == false)
        {

            lastBeatTime = Time.time - timeSinceLastPause;
            nextBeatTime = lastBeatTime + beatInterval;
            Playing = true;
        }

        else
        {
            timeSinceLastPause = Time.time - lastBeatTime;
            Playing = false;
        }

        OnTogglePlayPause?.Invoke();
    }

    public void SetPlayPauseMetronome(bool b)
    {
        if (b)
        {

            lastBeatTime = Time.time - timeSinceLastPause;
            nextBeatTime = lastBeatTime + beatInterval;
            Playing = true;
        }

        else
        {
            timeSinceLastPause = Time.time - lastBeatTime;
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

