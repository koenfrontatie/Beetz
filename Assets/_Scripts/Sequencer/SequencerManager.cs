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
        Events.OnBuildNewSequencer += BuildSequencer;
    }

    //private void BuildSequencer()
    //{
    //    var s = Instantiate(Prefabs.Instance.Sequencer, _gridManager.GetCenter(), Quaternion.identity, transform);
    //    s.Init(_gridManager.GetCenter(), DisplayType);
    //}

    public void BuildSequencer(Vector3 position, SequencerInfo info)
    {
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, position, Quaternion.identity, transform);
        newSequencer.Init(position, info);
        GridController.Instance.AddSequencerInformation(newSequencer);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.OnBuildNewSequencer -= BuildSequencer;
    }
}
