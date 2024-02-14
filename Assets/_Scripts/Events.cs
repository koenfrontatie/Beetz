using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
   public static Action<Vector3> OnLocationClicked;
   public static Action<Vector3> OnMouseRaycastGrid;
   public static Action<int> OnHotbarClicked;
   public static Action<GameState> OnGameStateChanged;
   //public static Action<Vector3Int> OnNewGridTarget;
}
