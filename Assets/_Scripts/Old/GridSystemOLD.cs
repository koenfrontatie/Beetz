using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemOLD : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] float tileSize;
    [SerializeField] private List<Vector3> positions;
    [SerializeField] private Grid grid;
    [SerializeField] Vector3Int worldToCell;
    [SerializeField] Bounds cellBounds;
    public void Generate()
    {
        positions.Clear();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = new Vector3(i, transform.localPosition.y, j) * tileSize;
                positions.Add(pos);
            }
        }
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            worldToCell = grid.WorldToCell(hit.point);
            cellBounds = grid.GetBoundsLocal(worldToCell, Vector3.one);
            //if (hit.transform.gameObject != null)
            //{
            //    worldToCell = grid.WorldToCell(hit.transform.position);
            //    //hit.transform.position;
            //}
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < positions.Count; i++)
        {
            Gizmos.DrawSphere(positions[i], 0.1f);
        }
    }
}
