using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Step : MonoBehaviour
{
    private SampleObject _sampleObject;
    private Sequencer _sequencer;
    public int BeatIndex;
    private Material _mat;

    private void OnDisable()
    {
        Metronome.NewStep -= CheckForPlayBack;
        Metronome.TogglePlayPause -= PlayPauseHandler;
    }
    private void Awake()
    {
        _mat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        transform.parent.parent.TryGetComponent<Sequencer>(out _sequencer);
    }

    private void Start()
    {
        Metronome.NewStep += CheckForPlayBack;
        Metronome.TogglePlayPause += PlayPauseHandler;
    }

    public void SetColor(Color c)
    {
        _mat.color = c;
    }
    public void AssignSample(SampleObject so)
    {
        _sampleObject = so;

        Events.OnGrowAnim?.Invoke(_sampleObject.gameObject);
    }

    public SampleObject GetSampleObject()
    {
        return (_sampleObject != null) ? _sampleObject : null;
    }

    public Sequencer GetSequencer()
    {
        return (_sequencer != null) ? _sequencer : null;
    }

    public void UnAssignSample()
    {
        if (_sampleObject == null) return;
        // Check if the SampleObject is a runtime instance
        //if (_sampleObject.gameObject.scene.IsValid())
        //{
            Destroy(_sampleObject.gameObject);
        //}
        _sampleObject = null;
    }

    public bool HasSampleObject()
    {
        return (_sampleObject != null) ?  true : false;
    }

    void CheckForPlayBack()
    {
        if (GameManager.Instance.State != GameState.Gameplay && GameManager.Instance.State != GameState.CircularEdit) return;

        if (_sampleObject == null || _sequencer == null) return;
        //Debug.Log($"step:{_sequencer.CurrentStep} sibling+1:{transform.GetSiblingIndex() + 1} rowamt:{_sequencer.RowAmount} s1%amt:{(_sequencer.CurrentStep) % _sequencer.StepAmount}");
        bool shouldPlay = _sequencer.CurrentStep - 1 == BeatIndex;
        
        if(GameManager.Instance.State == GameState.CircularEdit)
        {
            if(SequencerManager.Instance.LastInteracted.SequencerData.ID != _sequencer.SequencerData.ID)
            {
                shouldPlay = false;
            }

        }
        //if(!_sequencer.IsLooping) // global playback
        //{
        //    shouldPlay = false;
        //    if(_playback.PlaylistStep == BeatIndex )
        //    {
        //        shouldPlay = true;
        //    }
        //}

        if (!shouldPlay) return;
            SendScoreEvent();
            SendAnimEvent();
    }

    void PlayPauseHandler()
    {
        if (Metronome.Instance.BeatProgression != 0) return;

        CheckForPlayBack();
    }

    void SendScoreEvent()
    {
        //Events.LoadPlayGuid?.Invoke(_sampleObject.SampleData.ID);
        Events.LoadPlaySample?.Invoke(_sampleObject.SampleData);
        //CsoundController.Instance.SendEventToQueue

    }

    void SendAnimEvent()
    {
        //Debug.Log($"ANIMATE {_sampleObject}");
        Events.OnScaleBounce?.Invoke(_sampleObject.gameObject);
    }
}
