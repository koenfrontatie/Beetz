using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Events
{
   public static Action OnClicked;
   public static Action<Vector3> OnTouchRaycastMove;

   public static Action OnGridClicked;
   public static Action<Vector2, Vector2> OnNewSequencer;
   public static Action<Sequencer, int> OnSequencerClicked;

   public static Action<int> OnHotbarClicked;
   public static Action<SampleObject> OnSampleSelection;
   public static Action<GridState> OnGridStateChanged;
    //public static Action<Vector3Int> OnNewGridTarget;
    public static UnityAction<string> OnSampleTrigger;
    public static UnityAction OnBaseSamplesLoaded;
}
