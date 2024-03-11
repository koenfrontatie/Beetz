using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    public GridState State;

    [SerializeField] private SoilDrawer _soilDrawPrefab;
    [SerializeField] private BarDrawer _barDrawPrefab;
    [SerializeField] private GameObject _gridDisplay;
    [SerializeField] private RectTransform _dragger;

    private Vector2 _startCell, _currentCell, lastCellPosition, _drawerDimensions;
    private GridController _gridController;
    private bool _isDragging;
    
    private SoilDrawer _drawInstance;
    //private BarDrawer _dragBarInstance;
    private Camera _cam;
    private LayerMask _layerMask;
    private LeanFinger _finger;

    private void Start()
    {
        _gridController = GetComponent<GridController>();
        _layerMask += LayerMask.GetMask("Grid");
        _cam = Camera.main;
        State = GridState.Default;
    }
    private void OnEnable()
    {
        LeanTouch.OnFingerDown += FingerDownHandler;
        LeanTouch.OnFingerUp += FingerUpHandler;
        LeanTouch.OnFingerTap += FingerTapHandler;
        Events.OnSequencerHeld += SpawnDragBar;
        //Events.OnSequencerHeld += RemoveHeldSequencer;
        //Events.OnGridClicked += () => { if (_dragBarInstance != null) Destroy(_dragBarInstance.gameObject); };
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= FingerDownHandler;
        LeanTouch.OnFingerUp -= FingerUpHandler;
        Events.OnSequencerHeld -= SpawnDragBar;
        Events.OnSequencerHeld -= RemoveHeldSequencer;
        //Events.OnGridClicked -= () => { if (_dragBarInstance != null) Destroy(_dragBarInstance.gameObject); };
    }

    private void Update()
    {
        if (!DrawingAllowed()) return;

        // Handle drawing of sequencer

        _currentCell = RaycastFingerToCell(_finger.ScreenPosition);

        if (_currentCell == lastCellPosition) return;

        _drawerDimensions = new Vector3(Mathf.Clamp((_currentCell.x - _startCell.x) + 1, 1, 128), Mathf.Clamp(((_currentCell.y - _startCell.y) - 1 ) * -1, 1, 128), 0);

        UpdateDrawInstance();

        lastCellPosition = _currentCell;
    }

    /// <summary>
    /// Handles drag start.
    /// </summary>
    void FingerDownHandler(LeanFinger finger)
    {
        if (State != GridState.Patching) return;
        if (finger.IsOverGui) return;
        if (_isDragging) return;
        
        _finger = finger;
        
        _startCell = RaycastFingerToCell(finger.ScreenPosition);
        
        _drawInstance = Instantiate(_soilDrawPrefab, transform);
        UpdateDrawInstance();

        _isDragging = true;
        
    }
    /// <summary>
    /// Handles drag end.
    /// </summary>
    void FingerUpHandler(LeanFinger finger)
    {
        if (State != GridState.Patching || finger.IsOverGui) return;
        Events.OnNewSequencer?.Invoke(_startCell, _drawerDimensions);
        if (_drawInstance != null) Destroy(_drawInstance.gameObject);
        if (_isDragging) _isDragging = false;
        SetState(GridState.Default);
    }

    void FingerTapHandler(LeanFinger finger)
    {
        if (State == GridState.Default || finger.IsOverGui) return;
        if (Physics.Raycast(_cam.ScreenPointToRay(finger.ScreenPosition), out var hit, Mathf.Infinity, _layerMask))
        {

        SetState(GridState.Default);
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

    private void UpdateDrawInstance()
    {
        //Debug.Log($"CellPos = {StartCell}, WorldPos = {_gridController.WorldFromCell(StartCell)}");
        _drawInstance.DrawQuad(_gridController.WorldFromCell(_startCell), new Vector2(_drawerDimensions.x, _drawerDimensions.y));
    }

    private bool DrawingAllowed()
    {
        if (State != GridState.Patching || !_isDragging) return false;
        
        return true;
    }

    private void SpawnDragBar(Sequencer seq)
    {
        //if (_dragBarInstance != null) Destroy(_dragBarInstance.gameObject);
        //_dragBarInstance = Instantiate(_barDrawPrefab, seq.transform);
        ////_dragBarInstance.transform.position += Vector3.forward;
        //_dragBarInstance.DrawQuad(_gridController.WorldFromCell(seq.InstanceCellPosition) - Vector3.back * Config.CellSize, new Vector2(seq.StepAmount, 1));
        if (State == GridState.Moving) return;
        SetState(GridState.Moving);
        var pos = _cam.WorldToScreenPoint(seq.transform.position + Vector3.forward * Config.CellSize * 1.3f + new Vector3(Config.CellSize * (seq.StepAmount -1) * .5f, 0, 0));
        _dragger.transform.position = pos;
        //_dragger.transform.position = new Vector3(pos.x, pos.y, pos.z);
        //_dragger.setp
        Debug.Log($"{pos} {seq.transform.position}");
    }

    private void RemoveHeldSequencer(Sequencer seq)
    {
        Destroy(seq.gameObject);
        //Events.OnRemoveSequencer?.Invoke();
    }

    private void SetState(GridState state)
    {
        //switch (state)
        //{
        //    case GridState.Default:

        //        break;
        //    case GridState.Patching:

        //        break;
        //}

        _gridDisplay.gameObject.SetActive(state == GridState.Patching ? true : false);
        _dragger.gameObject.SetActive(state == GridState.Moving ? true : false);

        State = state;
    }

    public void NextState()
    {
        var newState = State.NextEnumValue();
        SetState(newState);
    }

    public void SetState(int i)
    {
        var state = (GridState)i;

        switch (state)
        {
            case GridState.Default:

                break;
            case GridState.Patching:

                break;
            case GridState.Moving:

                break;
        }

        _gridDisplay.gameObject.SetActive(state == GridState.Patching ? true : false);
        _dragger.gameObject.SetActive(state == GridState.Moving ? true : false);
        State = state;
    }

}

public enum GridState
{
    Default,
    Patching,
    Moving
}
