using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        Events.OnLocationClicked += BuildSequencer;
    }

    //private void BuildSequencer(Vector3 position, int stepAmount)
    //{
    //    Sequencer s = Instantiate(Prefabs.Instance.Sequencer, position, Quaternion.identity, transform);
    //    if (DisplayType == DisplayType.Linear) s.GetComponent<CircularDisplay>().enabled = false;
    //    if (DisplayType == DisplayType.Circular) s.GetComponent<GridDisplay>().enabled = false;
    //    s.SetSteps(stepAmount);
    //}

    private void BuildSequencer(Vector3 position)
    {
        Sequencer s = Instantiate(Prefabs.Instance.Sequencer, position, Quaternion.identity, transform);
        s.Init(position, 16, DisplayType);
    }

    public void ChangeDisplayType()
    {
        var typeCount = Enum.GetNames(typeof(DisplayType)).Length;
        var newInt = ((int)DisplayType + 1) % typeCount;
        //Debug.Log($"typecount={typeCount}, newInt={newInt}");
        DisplayType = (DisplayType)newInt;
    }
}
