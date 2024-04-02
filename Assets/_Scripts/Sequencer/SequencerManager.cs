using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public List<Sequencer> ActiveScarecrows = new List<Sequencer>();

    public DisplayType DisplayType;

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
    private void Start()
    {
        Events.BuildingSequencer += BuildSequencer;
        Events.CopyingSequencer += CloneSequencer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) BuildScarecrow(LastInteracted.SequencerData);
    }
    public void BuildSequencer(Vector3 worldPosition, SequencerData data)
    {
        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
        newSequencer.Init(worldPosition, data);
        ActiveSequencers.Add(newSequencer);
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

    public void BuildScarecrow(SequencerData data)
    {
        // make new data object for new sequencer
        var newID = data.ID;
        var copiedList = new List<PositionID>(data.PositionIDData);
        var copiedV2 = new Vector2(data.Dimensions.x, data.Dimensions.y);
        var newData = new SequencerData(newID, copiedV2, copiedList);

        var pos = new Vector3(0, 0, 8f);

        var newSequencer = Instantiate(Prefabs.Instance.CircularSequencer, pos, Quaternion.identity, transform);
        newSequencer.Init(pos, newData);

        ActiveScarecrows.Add(newSequencer);

        LastInteracted = newSequencer;
    }

    public void ChangeDisplayType() => DisplayType = DisplayType.NextEnumValue();

    private void OnDestroy()
    {
        Events.BuildingSequencer -= BuildSequencer;
        Events.CopyingSequencer -= CloneSequencer;
    }
}
