using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilDrawer : MonoBehaviour
{
    [SerializeField] private Transform _quadTransform;

    [SerializeField] private Vector2 _position;

    [SerializeField] private Vector2 _shape;

    void Start()
    {
        if (transform.parent.TryGetComponent<Sequencer>(out var seq))
        {
            var cellPos = GridManager.Instance.WorldPositionToCell(seq.gameObject.transform.position);
            Debug.Log(cellPos);
            InitFromSequencer(seq.transform.position, seq.SequencerInfo.Dimensions);
        }
    }


    public void Init()
    {
        //initializes from cell position
        var halfCell = Config.CellSize * .5f;
        var worldPos = new Vector3(halfCell + _position.x * Config.CellSize, 0f, halfCell + _position.y * Config.CellSize);
        var parentOffset = new Vector3(worldPos.x * Config.CellSize, 0f, worldPos.y * Config.CellSize + Config.CellSize);
        var quadOffset = new Vector3(-halfCell + Config.CellSize * _shape.x * .5f, 0f, Config.CellSize * _shape.y * -.5f + halfCell);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = worldPos + quadOffset;

        _quadTransform.localScale = new Vector3(_shape.x * quadScaleFactor, _shape.y * quadScaleFactor, 1f);

    }

    public void InitFromSequencer(Vector3 pos, Vector2 shape)
    {
        var parentOffset = new Vector3(pos.x * Config.CellSize, 0f, pos.y * Config.CellSize + Config.CellSize);
        var halfCell = Config.CellSize * .5f;
        var quadOffset = new Vector3(-halfCell + Config.CellSize * shape.x * .5f, 0f, Config.CellSize * shape.y * -.5f + halfCell);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = pos + quadOffset;

        _quadTransform.localScale = new Vector3(shape.x * quadScaleFactor, shape.y * quadScaleFactor , 1f);

        _position = GridManager.Instance.WorldPositionToCell(pos);
        _shape = shape;
    }
}
