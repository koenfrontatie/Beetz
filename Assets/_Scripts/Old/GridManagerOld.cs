//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class GridManagerOld : MonoBehaviour
//{
//    public static GridManagerOld Instance { get; private set; }

//    //public float CellSize;
//    public Vector3 CellCoords;

//    [SerializeField] private Grid grid;

//    [SerializeField] private Transform indicator;
//    [SerializeField] private float animTime;

//    private Vector3 lastCellPosition;
    
//    private void Awake()
//    {
//        Instance = this;   
//    }
//    private void OnEnable()
//    {
//        Events.OnTouchRaycastMove += UpdateCellPosition;
//        Events.OnGridStateChanged += ToggleIndicator;
//    }

//    private void OnDisable()
//    {
//        Events.OnTouchRaycastMove -= UpdateCellPosition;
//        Events.OnGridStateChanged -= ToggleIndicator;
//    }
//    void Start()
//    {
//        grid.cellSize = new Vector3(Config.CellSize, Config.CellSize, Config.CellSize);
//        indicator.transform.localScale = grid.cellSize;
//    }

//    private void Update()
//    {
//        if (EventSystem.current.IsPointerOverGameObject())
//        {
//            if (indicator.gameObject.activeSelf) indicator.gameObject.SetActive(false);
//        }
//        else
//        {
//            if (!indicator.gameObject.activeSelf) indicator.gameObject.SetActive(true);
//        }
//    }

//    private async void UpdateCellPosition(Vector3 mouseWorldPosition)
//    {
//        CellCoords = WorldPositionToCell(mouseWorldPosition);

//        if (CellCoords != lastCellPosition)
//        {
//            await Utils.LerpToTarget(indicator.gameObject, GetCenter(), animTime);
//            lastCellPosition = CellCoords;
//        }
//    }

//    private void ToggleIndicator(GameState state)
//    {
//        //cellPosition = lastCellPosition = WorldPositionToCell(mouseWorldPosition);
//        indicator.transform.position = CellCoords;
//        indicator.gameObject.SetActive(state == GameState.Patching ? true : false);
//    }
//    public Vector3 GetCenter()
//    {
//        return new Vector3(CellCoords.x * Config.CellSize + Config.CellSize * .5f, 0f, CellCoords.y * Config.CellSize + Config.CellSize * .5f);
//    }
    
//    public Vector3 WorldPositionToCell(Vector3 worldPosition)
//    {
//        return grid.WorldToCell(worldPosition);
//    }

//    public Vector3 CellPositionToWorld(Vector2 cell)
//    {
//        return grid.CellToWorld(new Vector3Int((int)cell.x, (int)cell.y, 0));
//    }
//}
