using Lean.Touch;
using System;
using UnityEngine;

public class LeanRaycaster : MonoBehaviour
{
    private LeanFinger _finger;
    private Camera _cam;
    private LayerMask _layerMask;

    private void Start()
    {
        _cam = Camera.main;
        _layerMask += LayerMask.GetMask("Sequencer");
        _layerMask += LayerMask.GetMask("Grid");
    }

    private void FingerUpHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            Events.OnFingerUp?.Invoke(hit.transform, hit.point);
        }
    }

    private void FingerHeldHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            Events.OnFingerHeld?.Invoke(hit.transform, hit.point);
        }
    }

    private void FingerDownHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            Events.OnFingerDown?.Invoke(hit.transform, hit.point);
        }
    }

    private void FingerTapHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            Events.OnFingerTap?.Invoke(hit.transform, hit.point);
        }
    }

    private void FingerUpdateHandler(LeanFinger finger)
    {
        Events.OnFingerUpdate?.Invoke(finger.ScreenPosition);
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerTap += FingerTapHandler;
        LeanTouch.OnFingerDown += FingerDownHandler;
        LeanTouch.OnFingerOld += FingerHeldHandler;
        LeanTouch.OnFingerUp += FingerUpHandler;
        LeanTouch.OnFingerUpdate += FingerUpdateHandler;
    }
    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= FingerTapHandler;
        LeanTouch.OnFingerDown -= FingerDownHandler;
        LeanTouch.OnFingerOld -= FingerHeldHandler;
        LeanTouch.OnFingerUp -= FingerUpHandler;
        LeanTouch.OnFingerUpdate -= FingerUpdateHandler;
    }
}
