using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    public InteractionState State;

    
    [SerializeField] private SoilDrawer _soilDrawPrefab;
    [SerializeField] private GameObject _gridDisplay;
    [SerializeField] private ContextColliders _contextColliders;

    private Sequencer _lastSequencer;
    private Vector2  _startCell, _currentCell, _lastCellPosition, _drawerDimensions;
    private GridController _gridController;
    private SoilDrawer _drawInstance;

    private Camera _cam;
    private LayerMask _layerMask;

    private void Start()
    {
        _gridController = GetComponent<GridController>();
        _layerMask += LayerMask.GetMask("Grid");
        _layerMask += LayerMask.GetMask("Sequencer");

        _cam = Camera.main;
        State = InteractionState.Default;
    }
    private void OnEnable()
    {
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

                    var index = transform.GetSiblingIndex();
                    Events.OnSequencerTapped?.Invoke(seq, index);
                }
                break;

            case InteractionState.Patching:
                if (transform.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                SetState(InteractionState.Default);

                break;

            case InteractionState.Moving:
                //if (transform.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
                //SetState(InteractionState.Default);
                //                                                    // todo : update sequencer and grid data 
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
            case InteractionState.Context:
                //------------------------------------------------------------------- start sequencer dragging
                if (t.gameObject.tag != "ContextDragger")
                {
                    SetState(InteractionState.Moving);
                    return;
                }
                
                _lastCellPosition = _gridController.CellFromWorld(t.position);
                _lastCellPosition = _gridController.CellFromWorld(t.position);
                SetState(InteractionState.Moving);

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

                if (_startCell != _currentCell)
                {
                    var info = DataManager.Instance.CreateNewSequencerInfo();
                    info.Dimensions = _drawerDimensions;
                    info.Type = DisplayType.Linear;
                    info.PositionIDPairs = new List<PositionIDPair>(info.PositionIDPairs);
                    Events.OnBuildNewSequencer?.Invoke(_gridController.GetCenterFromCell(_startCell), info);
                }

                SetState(InteractionState.Default);

                break;

            case InteractionState.Moving:
                _contextColliders.SetContextMenu(false);
                SetState(InteractionState.Default);
                // todo : update sequencer and grid data 
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
                    //------------------------------------------------------------------- open context menu 
                    //var step = t.GetSiblingIndex() + 1;
                    //var worldPosition = seq.transform.position + Vector3.forward * Config.CellSize * 2f + new Vector3(Config.CellSize * (seq.StepAmount - 1) * .5f, 0, 0);
                    //var screenPosition = _cam.WorldToScreenPoint(worldPosition);//+ Vector3.forward * Config.CellSize

                    _contextColliders.PositionHitboxes(seq);

                    _contextColliders.SetContextMenu(true);
                    //_draggerHitbox.transform.position = worldPosition;
                    //_draggerHitbox.transform.parent = seq.transform;
                    //_contextColliders.transform.position = screenPosition;
                    
                    _lastSequencer = seq;
                    
                    SetState(InteractionState.Context);
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

                if (_currentCell == _lastCellPosition) return;

                _drawerDimensions = new Vector3(Mathf.Clamp((_currentCell.x - _startCell.x) + 1, 1, 128), Mathf.Clamp(((_currentCell.y - _startCell.y) - 1) * -1, 1, 128), 0);
                // update preview
                _drawInstance.DrawQuad(_gridController.WorldFromCell(_startCell), new Vector2(_drawerDimensions.x, _drawerDimensions.y));

                _lastCellPosition = _currentCell;


                break;

            case InteractionState.Moving:
                // ------------------------------------------------------------------- move sequencer along grid
                _currentCell = RaycastFingerToCell(v2);

                if (_currentCell == _lastCellPosition) return;

                var delta = _currentCell - _lastCellPosition;

                var worlddelta = delta * Config.CellSize;

                _lastSequencer.transform.position += new Vector3(worlddelta.x, 0f, worlddelta.y);

                Events.OnSequencerMoved?.Invoke(_lastSequencer, delta);

                _lastCellPosition = _currentCell;

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
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving ? true : false);
        //_contextColliders.SetContextMenu((state == InteractionState.Context || state == InteractionState.Moving) ? true : false);



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
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving ? true : false);
        //_contextColliders.SetContextMenu((state == InteractionState.Context || state == InteractionState.Moving) ? true : false);

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
    Context,
    Patching,
    Moving
}
