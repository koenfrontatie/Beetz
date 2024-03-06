using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SampleObject _sampleObject;
    private Sequencer _sequencer;
    private int _beatIndex;

    private void OnEnable()
    {
        transform.parent.parent.TryGetComponent<Sequencer>(out _sequencer);

        Metronome.OnStep += CheckForPlayBack;
        Metronome.OnTogglePlayPause += PlayPauseHandler;
    }

    private void OnDisable()
    {
        Metronome.OnStep -= CheckForPlayBack;
        Metronome.OnTogglePlayPause -= PlayPauseHandler;
    }

    private void Start()
    {
        transform.parent.parent.TryGetComponent<Sequencer>(out _sequencer);
        _beatIndex = transform.GetSiblingIndex() % _sequencer.StepAmount;
    }
    public void SetMat(Material mat)
    {
        _meshRenderer.material = mat;
    }
    public void AssignSample(SampleObject so)
    {
        _sampleObject = so;
    }

    public void UnAssignSample()
    {
        if (_sampleObject == null) return;
        Destroy(_sampleObject.gameObject);
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
        bool shouldPlay = _sequencer.CurrentStep - 1 == _beatIndex;
        if (shouldPlay) 
        {
            //_sampleObject.PlayAudio();
            SendScoreEvent();
            SendAnimEvent();
        }
    }

    void PlayPauseHandler()
    {
        if (!Metronome.Instance.Playing || Metronome.Instance.BeatProgression != 0) return;

        CheckForPlayBack();
    }

    void SendScoreEvent()
    {
        CsoundController.Instance.CsoundUnity.SendScoreEvent($"i {(int)(_sampleObject.Info.Template + 1)} 0 6");
        //print($"i {(int)(_sampleObject.Info.Template + 1)} 0 6");
    }

    void SendAnimEvent()
    {
        Events.OnScaleBounce?.Invoke(_sampleObject.gameObject);
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CsoundEventSender : MonoBehaviour
//{
//    [SerializeField] SampleObject _so;
//    [SerializeField] CsoundUnity _cSound;

//    void Start()
//    {

//    }

//    void SendEvent(string template)
//    {
//        _cSound.SendScoreEvent($"i {_so.Info.Template - 1} 0 6");
//    }
//}
