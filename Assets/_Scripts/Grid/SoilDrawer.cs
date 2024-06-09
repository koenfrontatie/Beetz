
using UnityEngine;

public class SoilDrawer : MonoBehaviour
{
    [SerializeField] private Transform _quadTransform;

    [SerializeField] private Vector3 _positionWorld;

    [SerializeField] private Vector2 _dimensions;

    private Grid _grid;

    Sequencer seq;

    void Start()
    {
        //if(FindObjectOfType<Grid>());
        if (transform.parent.TryGetComponent<Sequencer>(out seq))
        {
            DrawQuad(seq.transform.position, seq.SequencerData.Dimensions);
            _quadTransform.position -= new Vector3(Config.CellSize * .5f, 0f, Config.CellSize * .5f);
        }
        Events.ResizeSequencer += OnResize;
    }

    private void OnResize(Vector2 vector, SequencerData data)
    {
        if (data.ID != seq.SequencerData.ID) return;

        transform.localPosition = Vector3.zero;
        //var offset = new Vector3(Config.CellSize, 0, 0) * .5f;
        DrawQuad(seq.transform.position - new Vector3(Config.CellSize * .5f, 0f, Config.CellSize * .5f), data.Dimensions);

        //InitFromSequencer(vector, data.Dimensions);
    }

    private void OnDisable()
    {
        Events.ResizeSequencer -= OnResize;
    }
    #region
    public void Init()
    {
        //initializes from cell position
        var halfCell = Config.CellSize * .5f;
        var worldPos = new Vector3(halfCell + _positionWorld.x * Config.CellSize, 0f, halfCell + _positionWorld.y * Config.CellSize);
        var parentOffset = new Vector3(worldPos.x * Config.CellSize, 0f, worldPos.y * Config.CellSize + Config.CellSize);
        var quadOffset = new Vector3(-halfCell + Config.CellSize * _dimensions.x * .5f, 0f, Config.CellSize * _dimensions.y * -.5f + halfCell);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = worldPos + quadOffset;

        _quadTransform.localScale = new Vector3(_dimensions.x * quadScaleFactor, _dimensions.y * quadScaleFactor, 1f);

    }

    public void InitFromSequencer(Vector3 pos, Vector2 shape)
    {
        var parentOffset = new Vector3(pos.x * Config.CellSize, 0f, pos.y * Config.CellSize + Config.CellSize);
        var halfCell = Config.CellSize * .5f;
        var quadOffset = new Vector3(-halfCell + Config.CellSize * shape.x * .5f, 0f, Config.CellSize * shape.y * -.5f + halfCell);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = pos + quadOffset;

        _quadTransform.localScale = new Vector3(shape.x * quadScaleFactor, shape.y * quadScaleFactor, 1f);

        _positionWorld = GridController.Instance.CellFromWorld(pos);
        _dimensions = shape;
    }
    #endregion

    public void DrawQuad(Vector3 pos, Vector2 dimensions)
    {
        //var parentOffset = new Vector3(pos.x, 0f, pos.y);
        
        
        var quadOffset = new Vector3( Config.CellSize * dimensions.x * .5f, 0f, (Config.CellSize * dimensions.y * -.5f) + Config.CellSize); // this allows quad to compensate for scaling (half of dimensions)

        _quadTransform.localPosition = quadOffset;

        transform.position = pos; // after calculating quad offset, set to top left corner for scaling, relative to child quad

        _quadTransform.localScale = new Vector3(dimensions.x * Config.CellSize, dimensions.y * Config.CellSize, 1f);
        
        _dimensions = dimensions;
    }
}
