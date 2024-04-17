using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDrawer : MonoBehaviour
{
    public List<Vector3> PathVectors = new List<Vector3>();

    public List<Vector3> PathPoints = new List<Vector3>();

    public Vector2 SpacingMinMax;

    public bool Closed;

    private int _instanceCount, _countPerDivision;

    public List<Vector3> GeneratePoints(List<Vector3> vectors)
    {
        PathPoints.Clear();
        PathVectors.Clear();

        PathVectors = vectors;

        var average = SpacingMinMax.x + SpacingMinMax.y * .5f;

        var pathLength = GetLength(PathVectors);

        _instanceCount = Mathf.RoundToInt(pathLength / average);

        _countPerDivision = Mathf.RoundToInt(_instanceCount / PathVectors.Count);

        float divisionLengthPercentage;

        for (int i = 0; i < PathVectors.Count - 1; i++)
        {
            divisionLengthPercentage = Vector3.Distance(PathVectors[i], PathVectors[i + 1]) / pathLength;

            for (int j = 0; j < _countPerDivision * divisionLengthPercentage; j++)
            {
                var inc = (j + 1) * Random.Range(SpacingMinMax.x, SpacingMinMax.y);

                var normalized = inc / Vector3.Distance(PathVectors[i], PathVectors[i + 1]);

                if (normalized > 1) continue;

                PathPoints.Add(Vector3.Lerp(PathVectors[i], PathVectors[i + 1], inc));
            }
        }

        if (!Closed) return PathPoints;

        divisionLengthPercentage = _countPerDivision * Vector3.Distance(PathVectors[PathVectors.Count - 1], PathVectors[0]) / pathLength;

        for (int j = 0; j < _countPerDivision * divisionLengthPercentage; j++)
        {
            var inc = (j + 1) * Random.Range(SpacingMinMax.x, SpacingMinMax.y);

            var normalized = inc / Vector3.Distance(PathVectors[PathVectors.Count - 1], PathVectors[0]);

            if (normalized > 1) continue;

            var point = Vector3.Lerp(PathVectors[PathVectors.Count - 1], PathVectors[0], inc);

            if (Vector3.Equals(point, PathVectors[0])) continue;

            PathPoints.Add(point);
        }

        return PathPoints;
    }

    private float GetLength(List<Vector3> pathVectors)
    {   
        float pathLength = 0;

        for (int i = 0; i < pathVectors.Count - 1; i++)
        {
            var distance = Vector3.Distance(pathVectors[i], pathVectors[i + 1]);
            pathLength += distance;
        }

        if (Closed) pathLength += Vector3.Distance(pathVectors[0], pathVectors[pathVectors.Count - 1]);

        return pathLength;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        for (int i = 0; i < PathPoints.Count; i++)
        {
            Gizmos.DrawSphere(PathPoints[i], .05f);
        }
        
        Gizmos.color = Color.red;

        for (int i = 0; i < PathVectors.Count; i++)
        {
            if(i > 0) { Gizmos.color = Color.yellow; }
            Gizmos.DrawSphere(PathVectors[i], .05f);
        }
    }

#endif
}