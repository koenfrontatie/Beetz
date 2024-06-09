using System.Collections.Generic;
using UnityEngine;

public class SequencerCanvas : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _selectedCanvas;
    [SerializeField] private Sequencer _sequencer;

    [SerializeField] private RectTransform _rect;

    [SerializeField] private RectTransform _timeLinePrefab;
    private List<RectTransform> _timeLines = new List<RectTransform>();
    void OnEnable()
    {
        Metronome.TempoChanged += UpdateRect;

        Events.ResizeSequencer += (v2, data) => UpdateRect();
    }

    private void Start()
    {
        UpdateRect();
    }

    void UpdateRect()
    {
        if (_sequencer == null || _rect == null) return; // Check for null

        var offset = new Vector3(-Config.CellSize, transform.localPosition.y, Config.CellSize) * .5f;
        
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

    public void ToggleSelect(bool selected)
    {
        ToggleCanvasGroup(_selectedCanvas, selected);
    }

    private void ToggleCanvasGroup(CanvasGroup canvasGroup, bool on)
    {
        canvasGroup.alpha = on ? 1 : 0;
        //canvasGroup.interactable =false;
        //canvasGroup.blocksRaycasts = false;
    }

    private void OnDisable()
    {
        Metronome.TempoChanged -= UpdateRect;
        Events.ResizeSequencer -= (v2, data) => UpdateRect();

    }
}
