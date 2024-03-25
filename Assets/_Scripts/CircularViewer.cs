//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CircularViewer : MonoBehaviour
//{
//    private void OnEnable()
//    {
//        Events.OnSendToScareCrow += CreateCopy;
//    }

//    private void OnDisable()
//    {
//        Events.OnSendToScareCrow -= CreateCopy;
//    }

//    void CreateCopy(Sequencer sequencer)
//    {
//        transform.DestroyChildren();


//        var info = DataHelper.Instance.CreateNewSequencerInfo();
//        info.Dimensions = sequencer.SequencerData.Dimensions;
//        info.Type = sequencer.SequencerData.Type;
//        info.PositionIDPairs = new List<PositionIDPair>(sequencer.SequencerData.PositionIDPairs);
//        Events.OnBuildNewSequencer?.Invoke(GridController.Instance.GetCenterFromCell(transform.position), info);
//    }
//}
