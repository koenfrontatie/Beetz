using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
   public static Action OnClicked;
   public static Action<Vector3> OnMouseRaycastMove;

   public static Action OnGridClicked;
   public static Action<Sequencer, int> OnSequencerClicked;

   public static Action<int> OnHotbarClicked;
   public static Action<SampleObject> OnSampleSelection;
   public static Action<GameState> OnGameStateChanged;
   //public static Action<Vector3Int> OnNewGridTarget;
}
