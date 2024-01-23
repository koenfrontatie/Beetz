using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public DisplayType DisplayType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InputController.OnLocationClicked += BuildSequencerAtPosition;
    }

    private void BuildSequencer(int stepAmount)
    {
        Sequencer s = Instantiate(Prefabs.Instance.Sequencer, transform, false);
        s.SetSteps(stepAmount);
    }

    private void BuildSequencerAtPosition(Vector3 position)
    {
        Sequencer s = Instantiate(Prefabs.Instance.Sequencer, position, Quaternion.identity, transform);
        if (DisplayType == DisplayType.Linear) s.GetComponent<CircularDisplay>().enabled = false;
        if (DisplayType == DisplayType.Circular) s.GetComponent<GridDisplay>().enabled = false;
        s.SetSteps(16);
    }
}
