using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControlEnabler : MonoBehaviour
{
    private GridInteraction _gridInteraction;
    private LeanDragCamera _dragCam;
    private LeanPinchCamera _pinchCamera;

    void Start()
    {
        _gridInteraction = FindObjectOfType<GridInteraction>();
        _dragCam = GetComponent<LeanDragCamera>();
        _pinchCamera = GetComponent<LeanPinchCamera>();
    }

   
    void Update()
    {
        if (_gridInteraction == null) return;
        if (_gridInteraction.State == GridState.Default && !_dragCam.enabled) _dragCam.enabled = true;
        if (_gridInteraction.State != GridState.Default && _dragCam.enabled) _dragCam.enabled = false;

    }
}
