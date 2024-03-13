using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    public InteractionState State;

    
    [SerializeField] private SoilDrawer _soilDrawPrefab;
    [SerializeField] private GameObject _gridDisplay, _draggerHitbox;
    [SerializeField] private RectTransform _dragger;

    private Vector2 _raycastScreenPosition, _startCell, _currentCell, lastCellPosition, _drawerDimensions;
    private GridController _gridController;
    private SoilDrawer _drawInstance;

    private Camera _cam;
    private LayerMask _layerMask;

    private void Start()
    {
        _gridController = GetComponent<GridController>();
        _layerMask += LayerMask.GetMask("Grid");
        _cam = Camera.main;
        State = InteractionState.Default;
    }
    private void OnEnable()
    {
        //Events.OnGridTapped += FingerTapHandler;
        //Events.OnNewRaycastScreenPosition += (v2) => _raycastScreenPosition = v2;
        //Events.OnDrag += DrawSequencer;
        //Events.OnGridFingerDown += FingerDownHandler;
        //Events.OnGridFingerUp += FingerUpHandler;
        //Events.OnSequencerHeld += SequencerHeldHandler;
        Events.OnFingerDown += FingerDownHandler;
        Events.OnFingerUp += FingerUpHandler;
        Events.OnFingerTap += FingerTapHandler;
        Events.OnFingerHeld += FingerHeldHandler;
        Events.OnFingerUpdate += FingerUpdateHandler;
    }

    private void FingerTapHandler(Transform transform, Vector3 vector)
    {
        switch (State)
        {
            case InteractionState.Default:
                if (transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
                {

                    var step = transform.GetSiblingIndex() + 1;
                    Events.OnSequencerTapped?.Invoke(seq, step);
                }
                break;

            case InteractionState.Patching:
                if (transform.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                SetState(InteractionState.Default);

                break;

            case InteractionState.Moving:
                if (transform.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                SetState(InteractionState.Default);
                break;
        }
    }

    private void OnDisable()
    {
        Events.OnFingerDown -= FingerDownHandler;
        Events.OnFingerUp -= FingerUpHandler;
        Events.OnFingerTap -= FingerTapHandler;
        Events.OnFingerHeld -= FingerHeldHandler;
        Events.OnFingerUpdate -= FingerUpdateHandler;
        //Events.OnGridTapped -= FingerTapHandler;
        //Events.OnNewRaycastScreenPosition -= (v2) => _raycastScreenPosition = v2;
        //Events.OnDrag -= DrawSequencer;
        //Events.OnGridFingerDown -= FingerDownHandler;
        //Events.OnGridFingerUp -= FingerUpHandler;
        //Events.OnSequencerHeld -= SequencerHeldHandler;
    }

    void FingerDownHandler(Transform t, Vector3 point)
    {
        switch(State)
        {
            case InteractionState.Default:

                break;

            case InteractionState.Patching:
                //------------------------------------------------------------------- start creating patch
                if (t.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                _startCell = _gridController.CellFromWorld(point);
                _drawInstance = Instantiate(_soilDrawPrefab, transform);
                _drawerDimensions = Vector2.one;
                // update preview
                _drawInstance.DrawQuad(t.position, new Vector2(_drawerDimensions.x, _drawerDimensions.y));

                break;

            case InteractionState.Moving: 
                
                break;
        }
    }

    void FingerUpHandler(Transform t, Vector3 point)
    {
        switch (State)
        {
            case InteractionState.Default:
                if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                break;

            case InteractionState.Patching:
                //------------------------------------------------------------------- build new sequencer
                //if (t.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                if(_startCell != _currentCell) Events.OnNewSequencer?.Invoke(_startCell, _drawerDimensions);

                SetState(InteractionState.Default);

                break;

            case InteractionState.Moving:

                break;
        }
    }

    void FingerHeldHandler(Transform t, Vector3 point)
    {
        switch (State)
        {
            case InteractionState.Default:
                if (t.parent.parent != null && t.parent.parent.TryGetComponent<Sequencer>(out var seq))
                {
                    //------------------------------------------------------------------- start sequencer dragging
                    //var step = t.GetSiblingIndex() + 1;
                    var worldPosition = seq.transform.position + Vector3.forward * Config.CellSize * 1.3f + new Vector3(Config.CellSize * (seq.StepAmount - 1) * .5f, 0, 0);
                    var screenPosition = _cam.WorldToScreenPoint(worldPosition);
                    
                    _draggerHitbox.transform.position = worldPosition;
                    _draggerHitbox.transform.parent = seq.transform;
                    _dragger.transform.position = screenPosition;
                    
                    SetState(InteractionState.Moving);
                }
                break;

            case InteractionState.Patching:
                

                break;

            case InteractionState.Moving:

                break;
        }
    }
    private void FingerUpdateHandler(Vector2 v2)
    {
        switch (State)
        {
            case InteractionState.Default:
                    
                break;

            case InteractionState.Patching:
                // ------------------------------------------------------------------- handle drawing of new sequencer

                if (_drawInstance == null) return;

                _currentCell = RaycastFingerToCell(v2);

                if (_currentCell == lastCellPosition) return;

                _drawerDimensions = new Vector3(Mathf.Clamp((_currentCell.x - _startCell.x) + 1, 1, 128), Mathf.Clamp(((_currentCell.y - _startCell.y) - 1) * -1, 1, 128), 0);
                // update preview
                _drawInstance.DrawQuad(_gridController.WorldFromCell(_startCell), new Vector2(_drawerDimensions.x, _drawerDimensions.y));

                lastCellPosition = _currentCell;


                break;

            case InteractionState.Moving:
                break;
        }
    }

    /// <summary>
    /// Raycasts touching finger to grid cell
    /// </summary>
    private Vector3 RaycastFingerToCell(Vector2 v2)
    {
        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {

            return _gridController.CellFromWorld(hit.point);
        }
        else
        {
            return Vector3.zero;
        }
    }


    private void SetState(InteractionState state)
    {
        //switch (state)
        //{
        //    case GridState.Default:

        //        break;
        //    case GridState.Patching:

        //        break;
        //}

        // enable/disable state dependent objects
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving? true : false);
        _dragger.gameObject.SetActive(state == InteractionState.Moving ? true : false);
        _draggerHitbox.gameObject.SetActive(state == InteractionState.Moving ? true : false);

        State = state;
    }

    public void SetState(int i)
    {
        var state = (InteractionState)i;

        switch (state)
        {
            case InteractionState.Default:

                break;
            case InteractionState.Patching:

                break;
            case InteractionState.Moving:

                break;
        }

        // enable/disable state dependent objects
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving? true : false);
        _dragger.gameObject.SetActive(state == InteractionState.Moving ? true : false);
        _draggerHitbox.gameObject.SetActive(state == InteractionState.Moving ? true : false);

        State = state;
    }
    public void NextState()
    {
        var newState = State.NextEnumValue();
        SetState(newState);
    }

    public void TogglePatching() {

        SetState( State == InteractionState.Patching ? InteractionState.Default : InteractionState.Patching);
    }
}

public enum InteractionState
{
    Default,
    Patching,
    Moving
}
