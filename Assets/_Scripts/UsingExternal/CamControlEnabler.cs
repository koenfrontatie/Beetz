using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControlEnabler : MonoBehaviour
{
    private GridInteraction _gridInteraction;
    [SerializeField]private LeanDragCamera _dragCam;
    [SerializeField]private LeanPinchCamera _pinchCamera;

    void Start()
    {
        _gridInteraction = FindObjectOfType<GridInteraction>();


    }

    private void OnEnable()
    {
        GameManager.StateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Library) _dragCam.enabled = false;
        if (state == GameState.Library) _pinchCamera.enabled = false;
        if (state == GameState.Biolab) _dragCam.enabled = false;
        if (state == GameState.Biolab) _pinchCamera.enabled = false;
        if (state == GameState.Gameplay) _dragCam.enabled = true;
        if (state == GameState.Gameplay) _pinchCamera.enabled = true;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnGameStateChanged;
    }

    void Update()
    {
        if (_gridInteraction == null) return;
        if (GameManager.Instance.State != GameState.Gameplay) return;

        if (_gridInteraction.State == InteractionState.Default && !_dragCam.enabled) _dragCam.enabled = true;
        if (_gridInteraction.State != InteractionState.Default && _dragCam.enabled) _dragCam.enabled = false;

    }
}
