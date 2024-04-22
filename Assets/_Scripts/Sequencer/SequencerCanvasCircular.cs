using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequencerCanvasCircular : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Sequencer _sequencer;

    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform _timeLinePrefab;

    [SerializeField] private CircularDisplayer _circularDisplayer;
    
    private List<RectTransform> _timeLines = new List<RectTransform>();
    void OnEnable()
    {
        Metronome.TempoChanged += UpdateDisplay;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {

        var radius = Vector3.Distance(_sequencer.transform.position, _circularDisplayer.Steps[_circularDisplayer.Steps.Count - 1].transform.position);

        _rect.sizeDelta = new Vector2(radius * 2, radius * 2);
        _rect.position = _circularDisplayer.transform.position;
        
        
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

            beatLine.sizeDelta = new Vector3(.1f, radius * Config.CellSize);
        }

    }

    private void OnDisable()
    {
        Metronome.TempoChanged -= UpdateDisplay;
    }
}
