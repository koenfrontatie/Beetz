using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerInteraction : MonoBehaviour
{
    private Camera _cam;
    private LayerMask _layerMask;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += FingerTapHandler;
        LeanTouch.OnFingerOld += FingerHeldHandler;
        //LeanTouch.OnFingerUp += FingerUpHandler;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= FingerTapHandler;
        LeanTouch.OnFingerOld -= FingerHeldHandler;
        //LeanTouch.OnFingerUp -= FingerUpHandler;
    }

    private void Start()
    {
        _cam = Camera.main;
        _layerMask += LayerMask.GetMask("Sequencer");

    }
    void FingerHeldHandler(LeanFinger finger)
    {
        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {

            if (hit.transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
            {

                var step = hit.transform.GetSiblingIndex() + 1;
                Events.OnSequencerHeld?.Invoke(seq);
            }
        }
    }
    void FingerTapHandler(LeanFinger finger)
    {
        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {

            if (hit.transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
            {

                var step = hit.transform.GetSiblingIndex() + 1;
                Events.OnSequencerTapped?.Invoke(seq, step);
            }
        }
    }
}
