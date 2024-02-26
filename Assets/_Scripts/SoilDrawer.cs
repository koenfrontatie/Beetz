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
    }
    //public void Init(Vector2 pos, Vector2 shape)
    //{
    //    _quadTransform.position = new Vector3(pos.x * Config.CellSize + Config.CellSize * .5f, 0f, pos.y * Config.CellSize + Config.CellSize * .5f);

    //    _quadTransform.localScale = new Vector3(   ( /*this is one x pixel*/ (shape.x * Config.CellSize)) * shape.x, ( /*this is one y pixel*/ (shape.y * Config.CellSize)) * shape.y, 1f) ;

    //    _quadTransform.localPosition = new Vector3(Config.CellSize * .5f + shape.x * Config.CellSize   , 0f, Config.CellSize * .5f  + shape.y * Config.CellSize);
    //}

    public void Init()
    {
        var parentOffset = new Vector3(_position.x * Config.CellSize, 0f, _position.y * Config.CellSize + Config.CellSize);
        transform.position = parentOffset; // set parent to top left corner

        _quadTransform.position = new Vector3(_position.x * Config.CellSize + Config.CellSize * .5f, 0f, _position.y * -Config.CellSize -Config.CellSize * .5f) + parentOffset; // set to initial cell position
        _quadTransform.localScale = new Vector3(Config.CellSize * _shape.x , -Config.CellSize * _shape.y , 1f); // apply scaling to quad
        //_quadTransform.position += new Vector3(Config.CellSize * _shape.x * .5f - Config.CellSize * .5f, 0f, Config.CellSize * _shape.y * .5f - Config.CellSize * _shape.y) + parentOffset; // adjust position after scale

        _quadTransform.position += new Vector3(Config.CellSize * _shape.x * .5f - Config.CellSize * .5f, 0f, _quadTransform.localScale.y * .5f + Config.CellSize * .5f); // adjust position after scale

    }
}
