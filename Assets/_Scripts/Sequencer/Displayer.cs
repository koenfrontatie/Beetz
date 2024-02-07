using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Displayer : MonoBehaviour
{
    public Sequencer sequencer { get; private set; }
    public List<Step> steps = new List<Step>();

    private void OnEnable()
    {
        Metronome.OnStep += UpdateMaterials;
        Metronome.OnResetMetronome += UpdateMaterials;
    }
    private void OnDisable()
    {
        Metronome.OnStep -= UpdateMaterials;
        Metronome.OnResetMetronome -= UpdateMaterials;
    }
    void Start()
    {
        sequencer = GetComponent<Sequencer>();
        PositionSteps();
    }

    abstract public void PositionSteps();
    //abstract public void ChangeSizing();
    public void UpdateMaterials()
    {
        for(int i = 0; i < steps.Count; i++)
        {
            if (i == sequencer.CurrentStep - 1)
            {
                steps[i].SetMat(Prefabs.Instance.Blue);
            } else
            {
                steps[i].SetMat(Prefabs.Instance.White);
            }
        }
    }
}

public enum DisplayType
{
    Linear,
    Circular
}
