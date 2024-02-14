using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public float CellSize;
    public Vector3 CellPosition;


    [SerializeField] private Transform indicator;
    [SerializeField] private float animTime;
    [SerializeField] private Grid grid;

    private Vector3 lastCellPosition;
    

    private void Awake()
    {
        Instance = this;   
    }
    void Start()
    {
        grid.cellSize = new Vector3(CellSize, CellSize, CellSize);

        Events.OnMouseRaycastGrid += UpdateCellPosition;
        Events.OnGameStateChanged += ToggleIndicator;
    }

    async void UpdateCellPosition(Vector3 mouseWorldPosition)
    {
        CellPosition = WorldPositionToCell(mouseWorldPosition);

        if (CellPosition != lastCellPosition)
        {
            await Utils.LerpToTarget(indicator.gameObject, GetCellCenter(CellPosition * CellSize), animTime);
            lastCellPosition = CellPosition;
        }
    }

    void ToggleIndicator(GameState state)
    {
        //cellPosition = lastCellPosition = WorldPositionToCell(mouseWorldPosition);
        indicator.transform.position = CellPosition;
        indicator.gameObject.SetActive(state == GameState.Placing ? true : false);
    }
    public Vector3 GetCenter()
    {
        return new Vector3(CellPosition.x * CellSize + CellSize * .5f, 0f, CellPosition.y * CellSize + CellSize * .5f);
    }
    public Vector3 GetCellCenter(Vector3 cellPosition)
    {
        return new Vector3(cellPosition.x + CellSize * .5f, 0, cellPosition.y + CellSize * .5f);
    }
    Vector3 WorldPositionToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }
}
