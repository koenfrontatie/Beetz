using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequencerCanvas : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Sequencer _sequencer;

    [SerializeField] private RectTransform _rect;

    [SerializeField] private RectTransform _timeLinePrefab;
    private List<RectTransform> _timeLines = new List<RectTransform>();
    void OnEnable()
    {
        Metronome.TempoChanged += UpdateRect;
    }

    private void Start()
    {
        UpdateRect();
    }

    void UpdateRect()
    {
        var offset = new Vector3(-Config.CellSize, 0f, Config.CellSize) * .5f;
        
        _rect.sizeDelta = _sequencer.SequencerData.Dimensions * Config.CellSize;
        _rect.position = _sequencer.InstancePosition;
        _rect.position += offset + new Vector3(_sequencer.SequencerData.Dimensions.x * Config.CellSize, 0, -_sequencer.SequencerData.Dimensions.y * Config.CellSize) * .5f;
        
        if(_timeLines.Count > 0 ) foreach (var line in _timeLines) { Destroy(line.gameObject); };

        _timeLines.Clear();

        var beatAmt = Mathf.FloorToInt(_sequencer.StepAmount / Metronome.Instance.StepsPerBeat);
        var beatDist = Metronome.Instance.StepsPerBeat * Config.CellSize;
        
        for (int i = 0; i <= beatAmt; i++)
        {
            var beatLine = Instantiate(_timeLinePrefab, _rect);
            _timeLines.Add(beatLine);

            var linePos = _rect.position - new Vector3((_sequencer.SequencerData.Dimensions.x * Config.CellSize * .5f), -.05f, (-_sequencer.SequencerData.Dimensions.y * Config.CellSize * .5f)) + new Vector3(beatDist, 0f, 0f) * i;

            beatLine.SetPositionAndRotation(linePos, _rect.rotation);

            beatLine.sizeDelta = new Vector3(.1f, _sequencer.SequencerData.Dimensions.y * Config.CellSize);
        }
    
    }

    private void OnDisable()
    {
        Metronome.TempoChanged -= UpdateRect;
    }
}
