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
            _sampleObject.PlayAudio();
        }
    }

    void PlayPauseHandler()
    {
        if (!Metronome.Instance.Playing || Metronome.Instance.BeatProgression != 0) return;

        CheckForPlayBack();
    }
}
