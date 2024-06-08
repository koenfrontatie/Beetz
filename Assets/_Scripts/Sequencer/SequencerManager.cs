using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public Sequencer LastInteracted;

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
    private void OnEnable()
    {
        Events.BuildingSequencer += BuildSequencer;
        Events.CopyingSequencer += CloneSequencer;
        Events.OpenNewProject += ClearAllSequencers;
    }

    public void BuildSequencer(Vector3 worldPosition, SequencerData data)
    {
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
        newSequencer.Init(worldPosition, data);
        LastInteracted = newSequencer;
    }

    public void CloneSequencer(Vector3 worldPosition, SequencerData data)
    {
        // make new data object for new sequencer
        var newID = SaveLoader.Instance.NewGuid();
        var copiedList = new List<PositionID>(data.PositionIDData);
        var copiedV2 = new Vector2(data.Dimensions.x, data.Dimensions.y);
        var newData = new SequencerData(newID, copiedV2, copiedList);
        
        BuildSequencer(worldPosition, newData);
    }

    public void ClearAllSequencers()
    {
        if (ActiveSequencers.Count == 0) return;
        //ActiveSequencers.Clear();   
        transform.DestroyChildren();
    }
    //public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDisable()
    {
        Events.BuildingSequencer -= BuildSequencer;
        Events.CopyingSequencer -= CloneSequencer;
        Events.OpenNewProject -= ClearAllSequencers;
    }
}
