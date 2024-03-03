using UnityEditor;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    [SerializeField] private int _xAmt;
    [SerializeField] private int _yAmt;

    private float _cellSize;

    [SerializeField] private LineMesh _lineMesh;


    public void UpdateGridMesh() {

#if UNITY_EDITOR
        transform.DestroyChildrenImmediate();
#else
        transform.DestroyChildren();
#endif
        _cellSize = Config.CellSize;

        for (int i = 0; i < _xAmt; i++)
        {
            var line = Instantiate(_lineMesh, transform, false);
            //line.transform.position -= new Vector3((_xAmt * _cellSize) / 2f, 0f, 0f) + new Vector3(_cellSize * i, 0f, 0f);
            line.transform.position = new Vector3((-_xAmt * _cellSize) / 2f, 0f, 0f) + new Vector3(_cellSize * i, 0f, 0f);
        }

        for (int i = 0; i < _yAmt; i++)
        {
            var line = Instantiate(_lineMesh, transform, false);
            line.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            //line.transform.position -= new Vector3((_xAmt * _cellSize) / 2f, 0f, 0f) + new Vector3(_cellSize * i, 0f, 0f);
            line.transform.position = new Vector3(0f, 0f, -_yAmt * _cellSize) / 2f + new Vector3(0f, 0f, _cellSize * i);
        }

    }
}

