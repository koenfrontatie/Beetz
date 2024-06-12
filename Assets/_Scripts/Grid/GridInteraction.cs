using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

public class GridInteraction : MonoBehaviour
{
    public InteractionState State;

    
    [SerializeField] private SoilDrawer _soilDrawPrefab;
    [SerializeField] private GameObject _gridDisplay;
    [SerializeField] private ContextColliders _contextColliders;

    [SerializeField] private CanvasGroup _contextHelp, _patchHelp;

    private Sequencer _lastSequencer;
    private Vector2  _startCell, _currentCell, _quadStartPosition, _lastCellPosition, _drawerDimensions, _posIdAdjustment;
    private GridController _gridController;
    private SoilDrawer _drawInstance;
    private int _colliderIndex;
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
                    var selectedGuid = FileManager.Instance.SelectedSampleGuid;
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
                            SampleObject instance = await AssetBuilder.Instance.GetSampleObject(selectedGuid);
                            //Debug.Log($"selectedGuid {selectedGuid}");
                            //var instance = Instantiate(await AssetBuilder.Instance.GetSampleObject(selectedGuid), matchingStep.transform);
                            instance.transform.SetParent(matchingStep.transform);
                            instance.transform.position = matchingStep.transform.position;
                            instance.SampleData.ID = selectedGuid;
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
                _drawInstance.DrawQuad(_startCell, new Vector2(_drawerDimensions.x, _drawerDimensions.y));
                //Debug.Log(t.position);


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
                    //Events.MovingCameraToScarecrow.Invoke();
                    SetState(InteractionState.Default);
                    GameManager.Instance.UpdateState(GameState.CircularEdit);
                    return;
                }

                if (t.gameObject.tag == "ContextResize")
                { 
                    //figure out which collider
                    _colliderIndex = _contextColliders.GetClosestCollider(point);
                    // 1 3 5 4 only one dimensional resize
                    // 0 2 4 6 diagonal
                    //if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                    //_drawInstance = Instantiate(_soilDrawPrefab, transform);
                    _drawInstance = _lastSequencer.GetComponentInChildren<SoilDrawer>();
                    _startCell = _lastSequencer.InstanceCellPosition;
                    
                    SetState(InteractionState.Resizing);

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
                //if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                break;

            case InteractionState.Patching:
                //------------------------------------------------------------------- build new sequencer
                if (_startCell != _currentCell)
                {
                    var sData = new SequencerData(SaveLoader.Instance.NewGuid(), _drawerDimensions, new List<PositionID>());
                    Events.BuildingSequencer?.Invoke(_gridController.GetCenterFromCell(_quadStartPosition), sData);
                    if (_drawInstance != null) Destroy(_drawInstance.gameObject);
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
                _contextColliders.SetContextMenu(false);
                Events.CopyingSequencer?.Invoke(_gridController.GetCenterFromCell(_currentCell), _lastSequencer.SequencerData);
                SetState(InteractionState.Default);
                break;
            case InteractionState.Resizing:

                var pid = new List<PositionID>();
                var adjustedDimensions = new Vector2(Mathf.Abs(_drawerDimensions.x), Mathf.Abs(_drawerDimensions.y));

                foreach (PositionID posid in _lastSequencer.SequencerData.PositionIDData)
                {
                    var adjusted = posid.Position + _posIdAdjustment;

                    if (adjusted.x > 0 && adjusted.x <= adjustedDimensions.x &&
                        adjusted.y > 0 && adjusted.y <= adjustedDimensions.y)
                    {
                        pid.Add(new PositionID(posid.ID, adjusted));
                    }
                }
                var newData = new SequencerData(_lastSequencer.SequencerData.ID, new Vector2(Mathf.Abs(_drawerDimensions.x), Mathf.Abs(_drawerDimensions.y)), pid);
                bool hInverted = _drawerDimensions.x < 0;
                bool vInverted = _drawerDimensions.y < 0;

                if (hInverted || vInverted)
                {
                    Vector2 invertedStart = Vector2.zero;
                    
                    if(hInverted && vInverted)
                    {
                        invertedStart = _quadStartPosition + new Vector2(_drawerDimensions.x, -_drawerDimensions.y);
                    }
                    else if (hInverted)
                    {
                        invertedStart = _quadStartPosition + new Vector2(_drawerDimensions.x, 0);
                    }
                    else if (vInverted)
                    {
                        invertedStart = _quadStartPosition + new Vector2(0, -_drawerDimensions.y);
                    }

                    //newData.PositionIDData.Clear();

                    Events.ResizeSequencer?.Invoke(invertedStart, newData);
                }
                else
                {
                    Events.ResizeSequencer?.Invoke(_quadStartPosition, newData);
                }
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

                var posCheck = (_currentCell.x >= _startCell.x, _currentCell.y >= _startCell.y);
                
                switch (posCheck)
                {
                    case (true, true):
                        DrawQuad((_currentCell.x - _startCell.x) + 1, (_currentCell.y - _startCell.y) + 1, _startCell.x, _currentCell.y);
                        break;
                    case (false, false):
                        DrawQuad((_startCell.x - _currentCell.x) + 1, (_startCell.y - _currentCell.y) + 1, _currentCell.x, _startCell.y);
                        break;
                    case (true, false):
                        DrawQuad((_currentCell.x - _startCell.x) + 1, (_startCell.y - _currentCell.y) + 1, _startCell.x, _startCell.y);
                        break;
                    case (false, true):
                        DrawQuad((_startCell.x - _currentCell.x) + 1, (_currentCell.y - _startCell.y) + 1, _currentCell.x, _currentCell.y);
                        break;
                }

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

            case InteractionState.Resizing:
                // ------------------------------------------------------------------- handle resizing of sequencer

                if (_drawInstance == null) return;

                _currentCell = RaycastFingerToCell(v2);

                if (_currentCell == _lastCellPosition) return;

                Vector2 difference = Vector2.zero;
                Vector2 adjustment = Vector2.zero;
                var halfpixelAdjust = new Vector3((Config.CellSize * .5f), 0, (Config.CellSize * .5f));

                switch (_colliderIndex)
                {
                    case 0:
                        difference = new Vector2(_lastSequencer.InstanceCellPosition.x - _currentCell.x, _lastSequencer.InstanceCellPosition.y - _currentCell.y);
                        adjustment = new Vector2(difference.x, -difference.y);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _currentCell.x, _currentCell.y);
                        _contextColliders.PositionHitboxes(new Vector3((_currentCell.x * Config.CellSize), 0, (_currentCell.y * Config.CellSize)) + halfpixelAdjust, _drawerDimensions);

                        break;
                    case 2:
                        difference = new Vector2(_currentCell.x - (_startCell.x + _lastSequencer.SequencerData.Dimensions.x), _currentCell.y - _startCell.y);
                        adjustment = new Vector2(difference.x + 1, difference.y);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _startCell.x, _currentCell.y);

                        _contextColliders.PositionHitboxes(new Vector3(_startCell.x * Config.CellSize, 0, _currentCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);

                        break;
                    case 4:
                        difference = new Vector2(_currentCell.x - (_startCell.x + _lastSequencer.SequencerData.Dimensions.x), (_startCell.y - _lastSequencer.SequencerData.Dimensions.y) - _currentCell.y);
                        adjustment = new Vector2(difference.x + 1, difference.y + 1);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _startCell.x, _startCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_startCell.x * Config.CellSize, 0, _startCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);

                        break;
                    case 6:
                        var lc = _lastSequencer.InstanceCellPosition - new Vector2(0, _lastSequencer.SequencerData.Dimensions.y);
                        difference = new Vector2(_lastSequencer.InstanceCellPosition.x - _currentCell.x, lc.y - _currentCell.y);
                        adjustment = new Vector2(difference.x, difference.y + 1);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _currentCell.x, _startCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_currentCell.x * Config.CellSize, 0, _startCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);
                        break;

                    // one dimensional resizing
                    case 1:
                        difference = new Vector2(0, _lastSequencer.InstanceCellPosition.y - _currentCell.y);
                        adjustment = new Vector2(0, -difference.y);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _startCell.x, _currentCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_startCell.x * Config.CellSize, 0, _currentCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);
                        break;
                    case 3:
                        difference = new Vector2(_currentCell.x - (_startCell.x + _lastSequencer.SequencerData.Dimensions.x), 0);
                        adjustment = new Vector2(difference.x + 1, 0);
                        _drawerDimensions = new Vector2(_lastSequencer.SequencerData.Dimensions.x + adjustment.x, _lastSequencer.SequencerData.Dimensions.y);
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _startCell.x, _startCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_startCell.x * Config.CellSize, 0, _startCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);
                        
                        break;
                    case 5:
                        difference = new Vector2(0, (_startCell.y - _lastSequencer.SequencerData.Dimensions.y) - _currentCell.y);
                        adjustment = new Vector2(0, difference.y + 1);
                        _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _startCell.x, _startCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_startCell.x * Config.CellSize, 0, _startCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);
                        break;
                    case 7:
                        difference = new Vector2(_lastSequencer.InstanceCellPosition.x - _currentCell.x, 0);
                        adjustment = new Vector2(difference.x, 0f);
                        _drawerDimensions = _drawerDimensions = _lastSequencer.SequencerData.Dimensions + adjustment;
                        DrawQuad(_drawerDimensions.x, _drawerDimensions.y, _currentCell.x, _startCell.y);
                        _contextColliders.PositionHitboxes(new Vector3(_currentCell.x * Config.CellSize, 0, _startCell.y * Config.CellSize) + halfpixelAdjust, _drawerDimensions);
                        break;
                }

                var xShift = _drawerDimensions.x - _lastSequencer.SequencerData.Dimensions.x;
                var yShift = _drawerDimensions.y - _lastSequencer.SequencerData.Dimensions.y;

                switch (_colliderIndex)
                {
                    case 0:
                        _posIdAdjustment = new Vector3(xShift, yShift);
                        break;
                    case 1:
                        _posIdAdjustment = new Vector3(xShift, yShift);
                        break;
                    case 2:
                        _posIdAdjustment = new Vector3(0, yShift);
                        break;
                    case 6:
                        _posIdAdjustment = new Vector3(xShift, 0);
                        break;
                    case 7:
                        _posIdAdjustment = new Vector3(xShift, yShift);
                        break;
                    default:
                        _posIdAdjustment = Vector2.zero;
                        break;
                }

                _lastCellPosition = _currentCell;
                break;
        }
    }

    private void DrawQuad(float width, float height, float startX, float startY)
    {
        _drawerDimensions = new Vector3(width, height, 0);
        _quadStartPosition = new Vector3(startX, startY);
        _drawInstance.DrawQuad(_gridController.WorldFromCell(_quadStartPosition), new Vector2(_drawerDimensions.x, _drawerDimensions.y));
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
        _contextHelp.alpha = _patchHelp.alpha = 0;

        switch (state)
        {
            case InteractionState.Default:
                _contextColliders.SetContextMenu(false);
                break;

            case InteractionState.Context:
                _contextColliders.SetContextMenu(true);
                _contextHelp.alpha = 1;
                _contextColliders.PositionHitboxes(_lastSequencer);
                break;
            case InteractionState.Resizing:
                _contextColliders.SetUpperIcons(false);
            break;
            case InteractionState.Patching:
                if (_drawInstance != null) _drawInstance = null;
                _patchHelp.alpha = 1;
                break;
        }

        // enable/disable state dependent objects
        _gridDisplay.gameObject.SetActive(state == InteractionState.Patching || state == InteractionState.Moving || state == InteractionState.Copying || state == InteractionState.Resizing ? true : false);
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
    Moving,
    Copying,
    Resizing
}
