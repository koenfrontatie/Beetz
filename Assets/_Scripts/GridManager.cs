using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public float CellSize;
    public Vector3 CellCoords;

    [SerializeField] private Grid grid;

    [SerializeField] private Transform indicator;
    [SerializeField] private float animTime;

    private Vector3 lastCellPosition;
    
    private void Awake()
    {
        Instance = this;   
    }
    private void OnEnable()
    {
        Events.OnMouseRaycastMove += UpdateCellPosition;
        Events.OnGameStateChanged += ToggleIndicator;
    }

    private void DisEnable()
    {
        Events.OnMouseRaycastMove -= UpdateCellPosition;
        Events.OnGameStateChanged -= ToggleIndicator;
    }
    void Start()
    {
        grid.cellSize = new Vector3(CellSize, CellSize, CellSize);
    }

    private async void UpdateCellPosition(Vector3 mouseWorldPosition)
    {
        CellCoords = WorldPositionToCell(mouseWorldPosition);

        if (CellCoords != lastCellPosition)
        {
            await Utils.LerpToTarget(indicator.gameObject, GetCenter(), animTime);
            lastCellPosition = CellCoords;
        }
    }

    private void ToggleIndicator(GameState state)
    {
        //cellPosition = lastCellPosition = WorldPositionToCell(mouseWorldPosition);
        indicator.transform.position = CellCoords;
        indicator.gameObject.SetActive(state == GameState.Placing ? true : false);
    }
    public Vector3 GetCenter()
    {
        return new Vector3(CellCoords.x * CellSize + CellSize * .5f, 0f, CellCoords.y * CellSize + CellSize * .5f);
    }
    
    public Vector3 WorldPositionToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }
}
