using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteraction : MonoBehaviour
{
    public GridState State;

    
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
        State = GridState.Default;
    }
    private void OnEnable()
    {
        Events.OnGridTapped += FingerTapHandler;
        Events.OnNewRaycastScreenPosition += (v2) => _raycastScreenPosition = v2;
        Events.OnDrag += DrawSequencer;
        Events.OnGridFingerDown += FingerDownHandler;
        Events.OnGridFingerUp += FingerUpHandler;
        Events.OnSequencerHeld += SequencerHeldHandler;
    }

    private void OnDisable()
    {
        Events.OnGridTapped -= FingerTapHandler;
        Events.OnNewRaycastScreenPosition -= (v2) => _raycastScreenPosition = v2;
        Events.OnDrag -= DrawSequencer;
        Events.OnGridFingerDown -= FingerDownHandler;
        Events.OnGridFingerUp -= FingerUpHandler;
        Events.OnSequencerHeld -= SequencerHeldHandler;
    }

    /// <summary>
    /// Handles drag start.
    /// </summary>
    void FingerDownHandler()
    {
        if (State == GridState.Patching)
        {
            //------------------------------------------------------------------- start dragging
            _startCell = RaycastFingerToCell(_raycastScreenPosition);

            _drawInstance = Instantiate(_soilDrawPrefab, transform);

            UpdateDrawInstance();

        }
    }
    /// <summary>
    /// Handles drag end.
    /// </summary>
    void FingerUpHandler()
    {
        if (State != GridState.Patching) return;

        Events.OnNewSequencer?.Invoke(_startCell, _drawerDimensions);
        
        if (_drawInstance != null) Destroy(_drawInstance.gameObject);
                
        SetState(GridState.Default);
    }

    void FingerTapHandler()
    {
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
        if (State != GridState.Patching) return;
        _drawInstance.DrawQuad(_gridController.WorldFromCell(_startCell), new Vector2(_drawerDimensions.x, _drawerDimensions.y));
    }

    private bool DrawingAllowed()
    {
        if (State != GridState.Patching) return false;

        return true;
    }

    private void DrawSequencer()
    {
        // Handle drawing of sequencer

        _currentCell = RaycastFingerToCell(_raycastScreenPosition);

        if (_currentCell == lastCellPosition) return;

        _drawerDimensions = new Vector3(Mathf.Clamp((_currentCell.x - _startCell.x) + 1, 1, 128), Mathf.Clamp(((_currentCell.y - _startCell.y) - 1) * -1, 1, 128), 0);

        UpdateDrawInstance();

        lastCellPosition = _currentCell;
    }

    private void SequencerHeldHandler(Sequencer seq)
    {
        if (State == GridState.Moving) return;
        SetState(GridState.Moving);
        var worldPosition = seq.transform.position + Vector3.forward * Config.CellSize * 1.3f + new Vector3(Config.CellSize * (seq.StepAmount - 1) * .5f, 0, 0);
        var screenPosition = _cam.WorldToScreenPoint(worldPosition);
        _draggerHitbox.transform.position = worldPosition;
        _draggerHitbox.transform.parent = seq.transform;
        _dragger.transform.position = screenPosition;
        //Debug.Log($"{screenPosition} {seq.transform.position}");
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
        _draggerHitbox.gameObject.SetActive(state == GridState.Moving ? true : false);

        State = state;
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

        _gridDisplay.gameObject.SetActive(state != GridState.Default ? true : false);
        _dragger.gameObject.SetActive(state == GridState.Moving ? true : false);
        _draggerHitbox.gameObject.SetActive(state == GridState.Moving ? true : false);
        State = state;
    }
    public void NextState()
    {
        var newState = State.NextEnumValue();
        SetState(newState);
    }

    public void TogglePatching() {

        SetState( State == GridState.Patching ? GridState.Default : GridState.Patching);
    }
}

public enum GridState
{
    Default,
    Patching,
    Moving
}
