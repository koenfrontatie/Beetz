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
        Events.OnBuildNewSequencer += BuildSequencer;
        Events.OnCopySequencer += CloneSequencer;
    }

    public void BuildSequencer(Vector3 worldPosition, SequencerInfo info)
    {
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
        newSequencer.Init(worldPosition, info);
        GridController.Instance.AddSequencerInformation(newSequencer);
    }

    public void CloneSequencer(Vector3 worldPosition, SequencerInfo info)
    {
        var newInfo = DataManager.Instance.CreateNewSequencerInfo();
        newInfo.Dimensions = info.Dimensions;
        newInfo.Type = info.Type;
        newInfo.PositionIDPairs = new List<PositionIDPair>(info.PositionIDPairs);
        Events.OnBuildNewSequencer?.Invoke(worldPosition, info);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.OnBuildNewSequencer -= BuildSequencer;
        Events.OnCopySequencer -= CloneSequencer;
    }
}
