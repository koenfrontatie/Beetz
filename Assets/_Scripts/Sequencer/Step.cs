using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Step : MonoBehaviour
{
    private SampleObject _sampleObject;
    private Sequencer _sequencer;
    private PlaylistPlayback _playback;
    public int BeatIndex;
    private Material _mat;

    private void OnDisable()
    {
        Metronome.OnStep -= CheckForPlayBack;
        Metronome.OnTogglePlayPause -= PlayPauseHandler;
    }
    private void Awake()
    {
        _mat = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        transform.parent.parent.TryGetComponent<Sequencer>(out _sequencer);
        transform.parent.parent.TryGetComponent<PlaylistPlayback>(out _playback);
    }

    private void Start()
    {
        Metronome.OnStep += CheckForPlayBack;
        Metronome.OnTogglePlayPause += PlayPauseHandler;
    }

    public void SetColor(Color c)
    {
        _mat.color = c;
    }
    public void AssignSample(SampleObject so)
    {
        _sampleObject = so;
    }

    public SampleObject GetSampleObject()
    {
        return (_sampleObject != null) ? _sampleObject : null;
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
        if (_sampleObject == null || _sequencer == null) return;
        //Debug.Log($"step:{_sequencer.CurrentStep} sibling+1:{transform.GetSiblingIndex() + 1} rowamt:{_sequencer.RowAmount} s1%amt:{(_sequencer.CurrentStep) % _sequencer.StepAmount}");
        bool shouldPlay = _sequencer.CurrentStep - 1 == BeatIndex;
        
        if(!_sequencer._isLooping) // global playback
        {
            shouldPlay = false;
            if(_playback.PlaylistStep == BeatIndex )
            {
                shouldPlay = true;
            }
        }

        if (!shouldPlay) return;
            SendScoreEvent();
            SendAnimEvent();
    }

    void PlayPauseHandler()
    {
        if (!Metronome.Instance.Playing || Metronome.Instance.BeatProgression != 0) return;

        CheckForPlayBack();
    }

    void SendScoreEvent()
    {
        Events.OnScoreEvent?.Invoke($"i {(int)(_sampleObject.SampleData.Template + 1)} 0 6");

    }

    void SendAnimEvent()
    {
        //Debug.Log($"ANIMATE {_sampleObject}");
        Events.OnScaleBounce?.Invoke(_sampleObject.gameObject);
    }
}
