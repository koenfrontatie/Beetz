using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

public class SplineDrawer : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

    [SerializeField] private float _paddingValue = .05f * Config.CellSize;

    float halfCell = Config.CellSize * .5f;


    void Start()
    {
        var border = _splineContainer.Spline.ToArray()[0];
        //if(FindObjectOfType<Grid>());
        if (transform.parent.TryGetComponent<Sequencer>(out var seq))
        {
            var offset = (Vector3.forward + Vector3.left) * halfCell;

            var knot0 = _splineContainer.Spline.ToArray()[0];
            knot0.Position = _splineContainer.transform.InverseTransformPoint(seq.transform.position) + offset + new Vector3(-_paddingValue, 0, _paddingValue);
            knot0.Rotation = Quaternion.Inverse(_splineContainer.transform.rotation) * seq.transform.rotation;
            _splineContainer.Spline.SetKnot(0, knot0);

            var knot1 = _splineContainer.Spline.ToArray()[1];
            knot1.Position = _splineContainer.transform.InverseTransformPoint(seq.transform.position + Vector3.right * seq.SequencerData.Dimensions.x) * Config.CellSize + offset + new Vector3(_paddingValue, 0, _paddingValue);
            knot1.Rotation = Quaternion.Inverse(_splineContainer.transform.rotation) * seq.transform.rotation;
            _splineContainer.Spline.SetKnot(1, knot1);

            var knot2 = _splineContainer.Spline.ToArray()[2];
            knot2.Position = _splineContainer.transform.InverseTransformPoint(seq.transform.position + Vector3.right * seq.SequencerData.Dimensions.x + Vector3.back * seq.SequencerData.Dimensions.y) * Config.CellSize + offset + new Vector3(_paddingValue, 0, -_paddingValue); ;
            knot2.Rotation = Quaternion.Inverse(_splineContainer.transform.rotation) * seq.transform.rotation;
            _splineContainer.Spline.SetKnot(2, knot2);

            var knot3 = _splineContainer.Spline.ToArray()[3];
            knot3.Position = _splineContainer.transform.InverseTransformPoint(seq.transform.position + Vector3.back * seq.SequencerData.Dimensions.y) * Config.CellSize + offset + new Vector3(-_paddingValue, 0, -_paddingValue);
            knot3.Rotation = Quaternion.Inverse(_splineContainer.transform.rotation) * seq.transform.rotation;
            _splineContainer.Spline.SetKnot(3, knot3);
        }

    }
}
