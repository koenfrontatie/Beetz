//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Dreamteck.Splines;

//public class RuntimeSpline : MonoBehaviour
//{
//    [SerializeField] GameObject _prefab;
//    [SerializeField] SplineComputer _pc;
//    [SerializeField] ObjectController _controller;
    
//    [SerializeField] private float _paddingValue = .05f * Config.CellSize;
//    float halfCell = Config.CellSize * .5f;
    
//    public List<Vector3> Knots;
//    public List<SplinePoint> Points;

//    private void Start()
//    {
//        if (transform.parent.TryGetComponent<Sequencer>(out var seq))
//        {
//            Knots.Clear();


//            var offset = (Vector3.forward + Vector3.left) * halfCell;

//            Knots.Add(_pc.transform.InverseTransformPoint(seq.transform.position) + offset + new Vector3(-_paddingValue, 0, _paddingValue));
//            Knots.Add(_pc.transform.InverseTransformPoint(seq.transform.position + Vector3.right * seq.SequencerData.Dimensions.x) * Config.CellSize + offset + new Vector3(_paddingValue, 0, _paddingValue));
//            Knots.Add(_pc.transform.InverseTransformPoint(seq.transform.position + Vector3.right * seq.SequencerData.Dimensions.x + Vector3.back * seq.SequencerData.Dimensions.y) * Config.CellSize + offset + new Vector3(_paddingValue, 0, -_paddingValue));
//            Knots.Add(_pc.transform.InverseTransformPoint(seq.transform.position + Vector3.back * seq.SequencerData.Dimensions.y) * Config.CellSize + offset + new Vector3(-_paddingValue, 0, -_paddingValue));

//            for (int i = 0; i < Knots.Count; i++)
//            {
//                Points.Add(new SplinePoint(Knots[i])
//                {
//                    normal = Vector3.up
//                });
//            }

//            _pc.SetPoints(Points.ToArray());

//            _pc.Close();


//            Debug.Log(_pc.CalculateLength() + " length dt spline");


//            _controller.spawnCount = 25;

//            _controller.Spawn();

//        }
//    }
//}
