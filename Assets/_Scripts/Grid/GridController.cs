using UnityEngine;

public class GridController : MonoBehaviour
{
    public static GridController Instance { get; private set; }
    public Grid Grid { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        Grid = GetComponentInChildren<Grid>();
        Grid.cellSize = new Vector3(Config.CellSize, Config.CellSize, Config.CellSize);
    }

    public Vector3 GetCenterFromCell(Vector2 cell)
    {
        var cellPos = new Vector3Int((int)cell.x, (int)cell.y, 0);
        var worldPos = Grid.CellToWorld(cellPos);

        return worldPos + new Vector3(Config.CellSize * .5f, 0f, Config.CellSize * .5f);
    }

    public Vector2 CellFromWorld(Vector3 worldPosition)
    {
        var pos = Grid.WorldToCell(worldPosition);
        return new Vector2(pos.x, pos.y);
    }

    public Vector3 WorldFromCell(Vector2 cell)
    {
        var cellPos = new Vector3Int((int)cell.x, (int)cell.y, 0);
        return Grid.CellToWorld(cellPos);
    }

}
