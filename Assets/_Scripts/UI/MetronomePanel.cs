using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MetronomePanel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _panel;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField]
    private TMP_Text _bpmPanel, _numeratorPanel, _denominatorPanel;

    [SerializeField] private int num;

    private void OnEnable()
    {
        Metronome.ResetMetronome += OnResetMetronome;
        Metronome.TempoChanged += OnResetMetronome;
    }

    private void OnResetMetronome()
    {
        _bpmPanel.text = Metronome.Instance.BPM.ToString();
        _inputField.text = Metronome.Instance.BPM.ToString();
        _numeratorPanel.text = Metronome.Instance.BeatsPerBar.ToString();
        _denominatorPanel.text = Metronome.Instance.StepsPerBeat.ToString();
    }

    public void Toggle()
    {
        _panel.ToggleCanvasGroup();
    }

    private void OnDisable()
    {
        Metronome.ResetMetronome -= OnResetMetronome;
        Metronome.TempoChanged -= OnResetMetronome;
    }
}
