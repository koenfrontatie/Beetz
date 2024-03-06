using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public DisplayType DisplayType;

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
        //GridController.Instance = FindObjectOfType<GridController>();
        Events.OnNewSequencer += BuildSequencer;
    }

    //private void BuildSequencer()
    //{
    //    var s = Instantiate(Prefabs.Instance.Sequencer, _gridManager.GetCenter(), Quaternion.identity, transform);
    //    s.Init(_gridManager.GetCenter(), DisplayType);
    //}

    public void BuildSequencer(Vector2 cell, Vector2 dimensions)
    {
        var spawnPos = GridController.Instance.GetCenterFromCell(cell);
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, spawnPos, Quaternion.identity, transform);
        newSequencer.Init(cell, dimensions);
        GridController.Instance.AddSequencerInformation(newSequencer);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.OnNewSequencer -= BuildSequencer;
    }
}
