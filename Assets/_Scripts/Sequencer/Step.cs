using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Step : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SampleObject _sampleObject;
    private Sequencer _sequencer;

    private void OnEnable()
    {
        transform.parent.TryGetComponent<Sequencer>(out _sequencer);

        Metronome.OnStep += CheckForPlayBack;
        Metronome.OnTogglePlayPause += PlayPauseHandler;
    }
    public void SetMat(Material mat)
    {
        _meshRenderer.material = mat;
    }
    public void AssignSample(SampleObject so)
    {
        _sampleObject = so;
    }

    void CheckForPlayBack()
    {
        if (_sampleObject == null || _sequencer == null) return;

        if(_sequencer.CurrentStep == transform.GetSiblingIndex() + 1)
        {
            //_sampleObject.PlayAudio();
            SendScoreEvent();
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
