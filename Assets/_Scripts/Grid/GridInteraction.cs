using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    public GridState State;

    [SerializeField] private SoilDrawer _drawPrefab;
    [SerializeField] private GameObject _gridDisplay;

    private Vector2 _startCell, _currentCell, lastCellPosition, _drawerDimensions;
    private GridController _gridController;
    private bool _isDragging;
    private SoilDrawer _drawInstance;
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
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= FingerDownHandler;
        LeanTouch.OnFingerUp -= FingerUpHandler;
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
        if (State == GridState.Default) return;
        if (finger.IsOverGui) return;
        if (_isDragging) return;
        
        _finger = finger;
        
        _startCell = RaycastFingerToCell(finger.ScreenPosition);
        
        _drawInstance = Instantiate(_drawPrefab, transform);
        UpdateDrawInstance();

        _isDragging = true;
        
    }
    /// <summary>
    /// Handles drag end.
    /// </summary>
    void FingerUpHandler(LeanFinger finger)
    {
        if (State == GridState.Default || finger.IsOverGui) return;
        Events.OnNewSequencer?.Invoke(_startCell, _drawerDimensions);
        if (_drawInstance != null) Destroy(_drawInstance.gameObject);
        if (_isDragging) _isDragging = false;
        SetState(GridState.Default);
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
        if (State == GridState.Default || !_isDragging) return false;
        
        return true;
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

        State = state;
    }

    public void NextState()
    {
        var newState = State.NextEnumValue();
        SetState(newState);
    }

}

public enum GridState
{
    Default,
    Patching
}
