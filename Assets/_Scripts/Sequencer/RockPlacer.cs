using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class RockPlacer : MonoBehaviour
{
    [SerializeField] List<GameObject> Prefabs;
    [SerializeField] List<float> Probabilities;

    [SerializeField] private float _paddingValue = .05f * Config.CellSize;

    [SerializeField] private PointDrawer _pointDrawer;

    private List<Vector3> _pathVectors = new List<Vector3>();
    private float halfCell = Config.CellSize * .5f;

    Sequencer _sequencer;

    private void Start()
    {
        if (transform.parent.TryGetComponent<Sequencer>(out _sequencer))
        {
            Place(_sequencer.transform.position, _sequencer.SequencerData.Dimensions);
        }
    }

    public void Place(Vector3 seqPosition, Vector2 seqDimensions)
    {
        _pathVectors.Clear();
        transform.DestroyChildren();
        var offset = (Vector3.forward + Vector3.left) * halfCell;
        //var _paddingVector = new Vector3(-_paddingValue, 0, _paddingValue);


        //_pathVectors.Add(transform.InverseTransformPoint(seqPosition) + offset + new Vector3(-_paddingValue, 0, _paddingValue));
        //_pathVectors.Add(transform.InverseTransformPoint(seqPosition) + Vector3.right * seqDimensions.x * Config.CellSize + offset + new Vector3(_paddingValue, 0, _paddingValue));
        //_pathVectors.Add(transform.InverseTransformPoint(seqPosition) + Vector3.right * seqDimensions.x + Vector3.back * seqDimensions.y * Config.CellSize + offset + new Vector3(_paddingValue, 0, -_paddingValue));
        //_pathVectors.Add(transform.InverseTransformPoint(seqPosition) + Vector3.back * seqDimensions.y * Config.CellSize + offset + new Vector3(-_paddingValue, 0, -_paddingValue));
        _pathVectors = new List<Vector3>();
        _pathVectors.Add((seqPosition) + offset + new Vector3(-_paddingValue, transform.position.y, _paddingValue));
        _pathVectors.Add((seqPosition) + Vector3.right * seqDimensions.x * Config.CellSize + offset + new Vector3(_paddingValue, transform.position.y, _paddingValue));
        _pathVectors.Add((seqPosition) + Vector3.right * seqDimensions.x * Config.CellSize + Vector3.back * seqDimensions.y * Config.CellSize + offset + new Vector3(_paddingValue, transform.position.y, -_paddingValue));
        _pathVectors.Add((seqPosition) + Vector3.back * seqDimensions.y * Config.CellSize + offset + new Vector3(-_paddingValue, transform.position.y, -_paddingValue));


        if (_pathVectors.Count < 2 ) { Debug.Log("not enough path vectors!"); return; }

        var points = _pointDrawer.GeneratePoints(_pathVectors);

        
            foreach (var point in points)
            {
                var obj = Instantiate(RandomObject(), point, Random.rotation, transform);
                obj.transform.localScale *= Random.Range(.9f, 1.3f); 
            }
       
    }

    public GameObject RandomObject()
    {
        // Calculate the cumulative probabilities
        List<float> cumulativeProbabilities = new List<float>();
        float sum = 0;
        foreach (float probability in Probabilities)
        {
            sum += probability;
            cumulativeProbabilities.Add(sum);
        }

        // Generate a random number between 0 and the sum of all probabilities
        float randomValue = Random.Range(0, sum);

        // Find the index where the random value falls within the cumulative probabilities
        int index = 0;
        while (randomValue > cumulativeProbabilities[index])
        {
            index++;
        }

        // Return the prefab at the found index
        return Prefabs[index];
    }
}
