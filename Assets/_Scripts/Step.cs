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
        if (_sampleObject != null || _sequencer == null) return;

        if(_sequencer.CurrentStep == transform.GetSiblingIndex() + 1)
        {
            Debug.Log($"sequencerstep is {_sequencer.CurrentStep} {transform.GetSiblingIndex() + 1}play {_sampleObject}!");
        }
    }

    //public void PlaceSample(SampleObject sample)
    //{
    //    var selectedSample = sample;

    //    if (sample == null) return;

    //    Instantiate(selectedSample, transform);
    //    // TODO save this data

    //    var pair = new PositionSamplePair();
    //    pair.ID = selectedSample.Info.ID;
    //    pair.Position = new Vector2(1, step + 1);
    //    Samples.Add(pair);
    //}

    //private void SequencerClickedHandler(Sequencer sequencer, int step)
    //{
    //    if (sequencer != this) return;

    //    var selectedSample = SampleManager.Instance.SelectedSample;

    //    if (SampleManager.Instance.SelectedSample == null) return;

    //    Instantiate(selectedSample, transform.GetChild(step).transform);
    //    // TODO save this data

    //    var pair = new PositionSamplePair();
    //    pair.ID = selectedSample.Info.ID;
    //    pair.Position = new Vector2(1, step + 1);
    //    Samples.Add(pair);
    //}
}
