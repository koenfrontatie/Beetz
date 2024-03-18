using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularViewer : MonoBehaviour
{
    private void OnEnable()
    {
        Events.OnSendToScareCrow += CreateCopy;
    }

    private void OnDisable()
    {
        Events.OnSendToScareCrow -= CreateCopy;
    }

    void CreateCopy(Sequencer sequencer)
    {
        transform.DestroyChildren();


        var info = DataManager.Instance.CreateNewSequencerInfo();
        info.Dimensions = sequencer.SequencerInfo.Dimensions;
        info.Type = sequencer.SequencerInfo.Type;
        info.PositionIDPairs = new List<PositionIDPair>(sequencer.SequencerInfo.PositionIDPairs);
        Events.OnBuildNewSequencer?.Invoke(GridController.Instance.GetCenterFromCell(transform.position), info);
    }
}
