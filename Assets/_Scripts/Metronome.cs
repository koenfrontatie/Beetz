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
   
    public static UnityAction NewBeat;
    public static UnityAction NewStep;

    public static UnityAction ResetMetronome;
    public static UnityAction<bool> PlayPause;
    public static UnityAction TogglePlayPause;
    public static UnityAction TempoChanged;

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
        Reset();
    }

    void Update()
    {
        if (Playing)
        {
            if (AudioSettings.dspTime >= nextBeatTime) 
            {
                // nextBeatTime is reached
                NewBeat?.Invoke();
                lastBeatTime = AudioSettings.dspTime;
                nextBeatTime = lastBeatTime + beatInterval;
            }

            // calculate beat progress
            BeatProgression = Mathf.Clamp01((float)(AudioSettings.dspTime - lastBeatTime) / beatInterval);
            
            // calculate step progress
            stepProgression = BeatProgression * StepsPerBeat % 1f;

            if (lastStepProgression > stepProgression) NewStep?.Invoke();
            lastStepProgression = stepProgression;
        }

        if (ToggleHalfTime)
        {
            Halftime = !Halftime;
            ToggleHalfTime = false;
        }

        TempoCheck();
    }

    public void Reset()
    {
        BeatProgression = 0;
        stepProgression = 0;
        lastBeatTime = AudioSettings.dspTime;
        beatInterval = Halftime ? 120f / BPM : 60f / BPM;

        ResetMetronome?.Invoke();
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

        TogglePlayPause?.Invoke();
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
            TempoChanged?.Invoke();
        }

        lastBPM = BPM;
        lastHalftime = Halftime;
    }

    public float GetStepProgression()
    {
        return stepProgression;
    }
}

