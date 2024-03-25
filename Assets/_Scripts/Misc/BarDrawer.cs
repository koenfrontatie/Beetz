using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarDrawer : MonoBehaviour
{
    [SerializeField] private Transform _quadTransform;

    [SerializeField] private Vector3 _positionWorld;

    [SerializeField] private Vector2 _dimensions;

    private Grid _grid;

    public void DrawQuad(Vector3 pos, Vector2 dimensions)
    {
        //var parentOffset = new Vector3(pos.x, 0f, pos.y);


        var quadOffset = new Vector3(Config.CellSize * dimensions.x * .5f, 0f, (Config.CellSize * dimensions.y * -.5f) + Config.CellSize); // this allows quad to compensate for scaling (half of dimensions)

        _quadTransform.localPosition = quadOffset;

        transform.position = pos; // after calculating quad offset, set to top left corner for scaling, relative to child quad

        _quadTransform.localScale = new Vector3(dimensions.x * Config.CellSize, dimensions.y * Config.CellSize, 1f);

        _dimensions = dimensions;
    }
}
