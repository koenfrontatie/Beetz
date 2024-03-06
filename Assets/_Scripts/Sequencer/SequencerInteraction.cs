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
        LeanTouch.OnFingerDown += FingerDownHandler;
        //LeanTouch.OnFingerUp += FingerUpHandler;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= FingerDownHandler;
        //LeanTouch.OnFingerUp -= FingerUpHandler;
    }

    private void Start()
    {
        _cam = Camera.main;
        _layerMask += LayerMask.GetMask("Sequencer");

    }

    void FingerDownHandler(LeanFinger finger)
    {
        RaycastFinger(finger.ScreenPosition);
    }

    private void RaycastFinger(Vector2 v2)
    {
        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {

            if (hit.transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
            {

                var step = hit.transform.GetSiblingIndex() + 1;
                Events.OnSequencerClicked?.Invoke(seq, step);
            }
        }
    }
}
