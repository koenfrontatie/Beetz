using UnityEngine;

public class ScareCrow : MonoBehaviour
{
    [SerializeField] private Sequencer _linkedSequencer;
    [SerializeField] private Transform _stick;
    Quaternion _startRotation;
    Quaternion _targetRotation;

    [SerializeField] private PlaybackController _controller;

    private float _rotationSpeed = 0.005f; // Adjust this value to control the speed of rotation
    private float _rotationProgress = 0f;

    private void Start()
    {
        _startRotation = _stick.transform.rotation;
        _targetRotation = _startRotation; // Initialize target rotation to start rotation
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            MakeScarecrow();
        }
    }

    void MakeScarecrow()
    {
        if (_linkedSequencer != null) Destroy(_linkedSequencer.gameObject);

        _linkedSequencer = Instantiate(Prefabs.Instance.CircularSequencer, transform.position, Quaternion.identity, transform);
        _linkedSequencer.Init(transform.position, SequencerManager.Instance.LastInteracted.SequencerData);

        ScarecrowManager.Instance.AddScarecrow(_linkedSequencer);
    }

    private void FixedUpdate()
    {
        if (_controller.PlaybackMode != PlaybackMode.Circular || _linkedSequencer == null) return;

        var totalSteps = _linkedSequencer.StepAmount;
        var degreesPerStep = 360f / totalSteps;

        var euler = new Vector3(0f, (_linkedSequencer.CurrentStep - 1) * degreesPerStep + Metronome.Instance.GetStepProgression() * degreesPerStep, 0f);
        _targetRotation = _startRotation * Quaternion.Euler(euler); // Calculate target rotation

        // Smoothly interpolate towards the target rotation
        _rotationProgress += Time.deltaTime * _rotationSpeed;
        _stick.rotation = Quaternion.Lerp(_stick.rotation, _targetRotation, _rotationProgress);
    }
}
