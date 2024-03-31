using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrow : MonoBehaviour
{
    [SerializeField] private Sequencer _linkedSequencer;
    [SerializeField] private Sequencer _scareCrowSequencer;

    private void OnEnable()
    {
        Events.SequencerBuilt += OnSequencerBuilt;
    }

    void OnSequencerBuilt(Sequencer seq)
    {
        CloneToScarecrow(transform.position, seq.SequencerData, DisplayType.Circular);
    }
    
    private void OnDisable()
    {
        Events.SequencerBuilt -= OnSequencerBuilt;
    }

    public void CloneToScarecrow(Vector3 worldPosition, SequencerData data, DisplayType type)
    {
        // make new data object for new sequencer
        var newID = data.ID;
        var copiedList = new List<PositionID>(data.PositionIDData);
        var copiedV2 = new Vector2(data.Dimensions.x, data.Dimensions.y);
        var newData = new SequencerData(newID, copiedV2, copiedList);

        var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
        newSequencer.Init(worldPosition, data, type);
    }
}
//var newSequencer = Instantiate(Prefabs.Instance.Sequencer, worldPosition, Quaternion.identity, transform);
//newSequencer.Init(worldPosition, data, type);