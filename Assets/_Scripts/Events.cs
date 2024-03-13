using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Events
{

   public static Action<Vector2, Vector2> OnNewSequencer;
   public static Action OnRemoveSequencer;
   public static Action<int> OnHotbarClicked;

   public static Action OnGridStateChanged;

    public static Action<string> OnScoreEvent;
    //public static Action<Vector3Int> OnNewGridTarget;
    public static UnityAction OnBaseSamplesLoaded;

    public static Action<GameObject> OnScaleBounce;


    // lean touch input events
    public static Action<Vector2> OnNewRaycastScreenPosition;
    public static Action OnDrag;

    public static Action OnGridTapped;
    public static Action OnGridFingerDown;
    public static Action OnGridFingerUp;
    public static Action<Sequencer> OnSequencerHeld;
    public static Action<Sequencer, int> OnSequencerTapped;

    public static Action<Transform, Vector3> OnFingerTap;
    public static Action<Transform, Vector3> OnFingerDown;
    public static Action<Transform, Vector3> OnFingerUp;
    public static Action<Transform, Vector3> OnFingerHeld;
    public static Action<Vector2> OnFingerUpdate;
}
