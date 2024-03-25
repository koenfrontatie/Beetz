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
        Events.BuildingSequencer += BuildSequencer;
        Events.CopyingSequencer += CloneSequencer;
    }

    public void BuildSequencer(Vector3 worldPosition, SequencerData data)
    {
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
        newSequencer.Init(worldPosition, data);
        GridController.Instance.AddSequencerInformation(newSequencer);
    }

    public void CloneSequencer(Vector3 worldPosition, SequencerData data)
    {
        var newData = new SequencerData(DataLoader.Instance.NewGuid(), data.Dimensions, data.PositionIDData);
        Events.BuildingSequencer?.Invoke(worldPosition, data);
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.BuildingSequencer -= BuildSequencer;
        Events.CopyingSequencer -= CloneSequencer;
    }
}
