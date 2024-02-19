using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public DisplayType DisplayType;

    private GridManager _gridManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
        Events.OnGridClicked += BuildSequencer;
    }

    private void BuildSequencer()
    {
        var s = Instantiate(Prefabs.Instance.Sequencer, _gridManager.GetCenter(), Quaternion.identity, transform);
        s.Init(_gridManager.GetCenter(), DisplayType);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.OnGridClicked -= BuildSequencer;
    }
}
