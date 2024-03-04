using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public DisplayType DisplayType;

    private GridController _gridController;

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
        _gridController = FindObjectOfType<GridController>();
        Events.OnNewSequencer += BuildSequencer;
    }

    //private void BuildSequencer()
    //{
    //    var s = Instantiate(Prefabs.Instance.Sequencer, _gridManager.GetCenter(), Quaternion.identity, transform);
    //    s.Init(_gridManager.GetCenter(), DisplayType);
    //}

    public void BuildSequencer(Vector2 cell, Vector2 dimensions)
    {
        var spawnPos = _gridController.GetCenterFromCell(cell);
        var s = Instantiate(Prefabs.Instance.Sequencer, spawnPos, Quaternion.identity, transform);
        s.Init(spawnPos, dimensions);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.OnNewSequencer -= BuildSequencer;
    }
}
