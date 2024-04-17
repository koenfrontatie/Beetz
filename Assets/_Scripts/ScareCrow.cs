using UnityEngine;

public class ScareCrow : MonoBehaviour
{
    [SerializeField] private Sequencer _linkedSequencer;
    [SerializeField] private Transform _stick;
    Quaternion _startRotation;
    Quaternion _targetRotation;

    [SerializeField] private PlaybackController _controller;
    [SerializeField] private float rotationSpeed = 10f; // Adjust this value to set the maximum rotation speed

    private void OnEnable()
    {
        Events.MakingScarecrow += MakeScarecrow;
    }

    private void OnDisable()
    {
        Events.MakingScarecrow -= MakeScarecrow;
    }
    private void Start()
    {
        _startRotation = _stick.transform.rotation;
        _targetRotation = _startRotation; // Initialize target rotation to start rotation
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        MakeScarecrow();
    //    }
    //}

    void MakeScarecrow()
    {
        if (_linkedSequencer != null) Destroy(_linkedSequencer.gameObject);

        _linkedSequencer = Instantiate(Prefabs.Instance.CircularSequencer, transform.position, Quaternion.identity, transform);
        _linkedSequencer.Init(transform.position, SequencerManager.Instance.LastInteracted.SequencerData);


        ScarecrowManager.Instance.AddScarecrow(_linkedSequencer);

    }

    //private void FixedUpdate()
    //{
    //    if (_controller.PlaybackMode != PlaybackMode.Circular || _linkedSequencer == null) return;

    //    var totalSteps = _linkedSequencer.StepAmount;
    //    var degreesPerStep = 360f / totalSteps;

    //    var euler = new Vector3(0f, (_linkedSequencer.CurrentStep - 1) * degreesPerStep + Metronome.Instance.GetStepProgression() * degreesPerStep, 0f);
    //    _targetRotation = _startRotation * Quaternion.Euler(euler); // Calculate target rotation

    //    _stick.rotation = _targetRotation;
    //}
    private void FixedUpdate()
    {
        if (_controller.PlaybackMode != PlaybackMode.Circular || _linkedSequencer == null) return;

        var totalSteps = _linkedSequencer.StepAmount;
        var degreesPerStep = 360f / totalSteps;

        var euler = new Vector3(0f, (_linkedSequencer.CurrentStep - 1) * degreesPerStep + Metronome.Instance.GetStepProgression() * degreesPerStep, 0f);
        _targetRotation = _startRotation * Quaternion.Euler(euler); // Calculate target rotation

        // Use Quaternion.RotateTowards to smoothly rotate towards the target rotation
        _stick.rotation = Quaternion.RotateTowards(_stick.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
    }

}
