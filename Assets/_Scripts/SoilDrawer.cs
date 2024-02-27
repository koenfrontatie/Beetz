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
        //Init(_position, _shape);
        //yield return new WaitForSeconds(.1f);
        if (transform.parent.TryGetComponent<Sequencer>(out var seq))
        {
            var cellPos = GridManager.Instance.WorldPositionToCell(seq.gameObject.transform.position);
            Debug.Log(cellPos);
            Init(new Vector2(cellPos.x, cellPos.y) * Config.CellSize, new Vector2(seq.StepAmount, seq.RowAmount));
        }
    }
    //public void Init(Vector2 pos, Vector2 shape)
    //{
    //    _quadTransform.position = new Vector3(pos.x * Config.CellSize + Config.CellSize * .5f, 0f, pos.y * Config.CellSize + Config.CellSize * .5f);

    //    _quadTransform.localScale = new Vector3(   ( /*this is one x pixel*/ (shape.x * Config.CellSize)) * shape.x, ( /*this is one y pixel*/ (shape.y * Config.CellSize)) * shape.y, 1f) ;

    //    _quadTransform.localPosition = new Vector3(Config.CellSize * .5f + shape.x * Config.CellSize   , 0f, Config.CellSize * .5f  + shape.y * Config.CellSize);
    //}

    //public void Init()
    //{
    //    var parentOffset = new Vector3(_position.x * Config.CellSize, 0f, _position.y * Config.CellSize + Config.CellSize);
    //    transform.position = parentOffset; // set parent to top left corner

    //    _quadTransform.position = new Vector3(_position.x * Config.CellSize + Config.CellSize * .5f, 0f, _position.y * -Config.CellSize -Config.CellSize * .5f) + parentOffset; // set to initial cell position
    //    _quadTransform.localScale = new Vector3(Config.CellSize * _shape.x , -Config.CellSize * _shape.y , 1f); // apply scaling to quad

    //    _quadTransform.position += new Vector3(Config.CellSize * _shape.x * .5f - Config.CellSize * .5f, 0f, _quadTransform.localScale.y * .5f ); // adjust position after scale

    //    //_quadTransform.position += new Vector3(Config.CellSize * _shape.x  - Config.CellSize * .5f, 0f, Config.CellSize * _shape.y - Config.CellSize * .5f); // adjust position after scale

    //}

    public void Init()
    {
        var parentOffset = new Vector3(_position.x * Config.CellSize, 0f, _position.y * Config.CellSize + Config.CellSize);
        var halfCell = Config.CellSize * .5f;
        var halfCellOffset = new Vector3(halfCell * _shape.x, 0f, -halfCell * _shape.y);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = new Vector3(_position.x * Config.CellSize, 0f, _position.y * Config.CellSize) + parentOffset + halfCellOffset; 

        _quadTransform.localScale = new Vector3(_shape.x * quadScaleFactor, _shape.y * quadScaleFactor, 1f);
    }

    public void Init(Vector2 pos, Vector2 shape)
    {
        var parentOffset = new Vector3(pos.x * Config.CellSize, 0f, pos.y * Config.CellSize + Config.CellSize);
        var halfCell = Config.CellSize * .5f;
        var halfCellOffset = new Vector3(halfCell * shape.x, 0f, -halfCell * shape.y);
        var quadScaleFactor = Config.CellSize;

        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = new Vector3(pos.x * Config.CellSize, 0f, pos.y * Config.CellSize) + parentOffset + halfCellOffset;

        _quadTransform.localScale = new Vector3(shape.x * quadScaleFactor, shape.y * quadScaleFactor, 1f);

    }
}
