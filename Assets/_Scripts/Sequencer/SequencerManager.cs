using System;
using System.Collections.Generic;
using UnityEngine;

public class SequencerManager : MonoBehaviour
{
    public static SequencerManager Instance;
    public List<Sequencer> ActiveSequencers = new List<Sequencer>();
    public DisplayType DisplayType;
    
    [SerializeField] private GridManager gridManager;

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

    private void BuildSequencer(Vector3 position)
    {
        Sequencer s = Instantiate(Prefabs.Instance.Sequencer, gridManager.GetCenter(), Quaternion.identity, transform);
        s.Init(position, 16, DisplayType);
    }

    public void ChangeDisplayType()
    {
        // iterates over type values
        DisplayType = DisplayType.NextEnumValue();
        
        //var typeCount = Enum.GetNames(typeof(DisplayType)).Length;
        //var newInt = ((int)DisplayType + 1) % typeCount;
        //DisplayType = (DisplayType)newInt;
    }

    private void OnDestroy()
    {
        Events.OnLocationClicked -= BuildSequencer;
    }
}
