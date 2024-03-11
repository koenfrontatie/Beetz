using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistPlayback : MonoBehaviour
{
    [SerializeField] private Sequencer _sequencer;
    private GridCounter _counter;
    public int PlaylistStep { get; private set; }
    private void Awake()
    {
        _sequencer = GetComponent<Sequencer>();
    }
    private void OnEnable()
    {
        Metronome.OnResetMetronome += () => ListenForPlayback();
        Metronome.OnStep += ListenForPlayback;
    }
    private void OnDisable()
    {
        Metronome.OnStep -= ListenForPlayback;
    }
    private void Start()
    {
        _counter = GridController.Instance.GridCounter;
        ListenForPlayback();
    }

    bool CounterIsInRange()
    {
        if (_counter.CurrentStep >= _sequencer.InstanceCellPosition.x && _counter.CurrentStep < _sequencer.InstanceCellPosition.x + _sequencer.StepAmount)
        {
            return true;
        }
            PlaylistStep = -1;
        return false;
    }

    void ListenForPlayback()
    {
        if (!CounterIsInRange()) return;

        PlaylistStep = _counter.CurrentStep - (int)_sequencer.InstanceCellPosition.x;
    }
}
