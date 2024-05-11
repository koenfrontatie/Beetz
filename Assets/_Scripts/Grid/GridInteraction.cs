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

    [SerializeField]private CanvasGroup _toolTip;

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

    private async void FingerTapHandler(Transform transform, Vector3 vector)
    {
        switch (State)
        {
            case InteractionState.Default:
                if (transform.TryGetComponent<Step>(out var tappedStep))
                {
                    var tappedSequencer = tappedStep.GetSequencer();
                    var data = tappedSequencer.SequencerData;

                    var index = transform.GetSiblingIndex();
                    var selectedGuid = AssetBuilder.Instance.SelectedGuid;
                    //var selectedGuid = AssetBuilder.Instance.SelectedSampleObject;
                    
                    if (string.IsNullOrEmpty(selectedGuid)) return;

                    // ------ update referenced sequencer data class
                    if (tappedStep.GetSampleObject() != null)
                    {
                        foreach (PositionID pair in data.PositionIDData)
                        {
                            if (pair.Position == tappedSequencer.GetPositionFromSiblingIndex(index))
                            {
                                data.PositionIDData.RemoveAt(data.PositionIDData.IndexOf(pair));
                                break;
                            }
                        }
                    } else
                    {

                        var posID = new PositionID(selectedGuid, tappedSequencer.GetPositionFromSiblingIndex(index));
                        data.PositionIDData.Add(posID);
                    }

                    // ------   loops through all sequencers and assigns samples on all matching id's
                    for (int i = 0; i < SequencerManager.Instance.ActiveSequencers.Count; i++) {
             
                        var matchingSequencer = SequencerManager.Instance.ActiveSequencers[i];
             
                        if (matchingSequencer.SequencerData.ID != tappedSequencer.SequencerData.ID) continue;

                        var matchingStep = matchingSequencer.Displayer.GetStepFromIndex(index);

                        if (matchingStep.GetSampleObject() != null)
                        {
                            //Debug.Log("Sample object is not null");

                            matchingStep.UnAssignSample();

                        } else
                        {
                            //Debug.Log("Instantiating new object");
                            var instance = await AssetBuilder.Instance.GetSampleObject(selectedGuid);
                            //var instance = Instantiate(await AssetBuilder.Instance.GetSampleObject(selectedGuid), matchingStep.transform);
                            instance.transform.SetParent(matchingStep.transform);
                            instance.transform.position = matchingStep.transform.position;

                            matchingStep.AssignSample(instance);

                            Events.SampleSpawned?.Invoke(instance.transform.position);
                        }
                    }

                    SequencerManager.Instance.LastInteracted = tappedSequencer;
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
            
            case InteractionState.Context:
                //if (transform.gameObject.layer != LayerMask.NameToLayer("Grid")) return;
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
                if (t.gameObject.tag == "ContextDrag")
                {
                    _lastCellPosition = _gridController.CellFromWorld(t.position);
                    _lastCellPosition = _gridController.CellFromWorld(t.position);
                    SetState(InteractionState.Moving);

                    return;
                }
                //------------------------------------------------------------------- remove sequencer
                if (t.gameObject.tag == "ContextRemove")
                {
                    Destroy(_lastSequencer.gameObject);
                    ScarecrowManager.Instance.CheckRemove(_lastSequencer);
                    SetState(InteractionState.Default);

                    return;
                }
                //------------------------------------------------------------------- start copying patch
                if (t.gameObject.tag == "ContextCopy")
                {
                    _startCell = _gridController.CellFromWorld(point);
                    _drawInstance = Instantiate(_soilDrawPrefab, transform);
                    _drawerDimensions = _lastSequencer.SequencerData.Dimensions;
                    // update preview
                    _drawInstance.DrawQuad(t.position, _lastSequencer.SequencerData.Dimensions);

                    SetState(InteractionState.Copying);

                    return;
                }

                if (t.gameObject.tag == "ContextScarecrow")
                {
                    Events.MakingScarecrow?.Invoke();
                    Events.MovingCameraToScarecrow.Invoke();
                    SetState(InteractionState.Default);

                    return;
                }

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
                    var sData = new SequencerData(SaveLoader.Instance.NewGuid(), _drawerDimensions, new List<PositionID>());
                    Events.BuildingSequencer?.Invoke(_gridController.GetCenterFromCell(_startCell), sData);
                }

                SetState(InteractionState.Default);

                break;

            case InteractionState.Moving:
                SetState(InteractionState.Default);
                // todo : update sequencer and grid data 
                break;
            case InteractionState.Copying:
                //------------------------------------------------------------------- build copy
                if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                Events.CopyingSequencer?.Invoke(_gridController.GetCenterFromCell(_currentCell), _lastSequencer.SequencerData);
                SetState(InteractionState.Default);
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
                    if (!t.parent.parent.TryGetComponent<LinearDisplayer>(out _)) return;
                    
                        //------------------------------------------------------------------- open context menu 
                        //var step = t.GetSiblingIndex() + 1;
                        //var worldPosition = seq.transform.position + Vector3.forward * Config.CellSize * 2f + new Vector3(Config.CellSize * (seq.StepAmount - 1) * .5f, 0, 0);
                        //var screenPosition = _cam.WorldToScreenPoint(worldPosition);//+ Vector3.forward * Config.CellSize
                        //_draggerHitbox.transform.position = worldPosition;
                        //_draggerHitbox.transform.parent = seq.transform;
                        //_contextColliders.transform.position = screenPosition;
                        SequencerManager.Instance.LastInteracted = seq;

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

                Events.MoveSequencer?.Invoke(_lastSequencer, delta);

                _lastCellPosition = _currentCell;

                break;
            case InteractionState.Copying:
                // ------------------------------------------------------------------- handle placement of copy
                if (_drawInstance == null) return;

                _currentCell = RaycastFingerToCell(v2);

                if (_currentCell == _lastCellPosition) return;

                //Events.OnSequencerMoved?.Invoke(_lastSequencer, _currentCell - _lastCellPosition); // params are not necessary i think

                // update preview
                _drawInstance.DrawQuad(_gridController.WorldFromCell(_currentCell), _lastSequencer.SequencerData.Dimensions);

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
        switch (state)
        {
            case InteractionState.Default:
                _contextColliders.SetContextMenu(false);
                break;
            case InteractionState.Context:
                _contextColliders.SetContextMenu(true);
                _contextColliders.PositionHitboxes(_lastSequencer);
                break;
            case InteractionState.Moving:
                break;
        }

        // enable/disable state dependent objects
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving || state == InteractionState.Copying ? true : false);
        //_contextColliders.SetContextMenu((state == InteractionState.Context || state == InteractionState.Moving) ? true : false);

        _toolTip.alpha = (state == InteractionState.Patching ? 1 : 0);


        State = state;
    }

    public void SetState(int i)
    {
        var state = (InteractionState)i;

        switch (state)
        {
            case InteractionState.Default:
                _contextColliders.SetContextMenu(false);
                //_toolTip.alpha = 0;
                break;
            case InteractionState.Context:
                _contextColliders.SetContextMenu(true);
                _contextColliders.PositionHitboxes(_lastSequencer);
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
        //_toolTip.alpha = (State == InteractionState.Patching ? 1 : 0);
    }
}

public enum InteractionState
{
    Default,
    Context,
    Patching,
    Moving,
    Copying
}
