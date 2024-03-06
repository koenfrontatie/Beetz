using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Displayer : MonoBehaviour
{
    public Sequencer Sequencer { get; private set; }
    public List<Step> Steps = new List<Step>();
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
        Sequencer = GetComponent<Sequencer>();
        PositionSteps();
    }

    abstract public void PositionSteps();
    //abstract public void ChangeSizing();
    public void UpdateMaterials()
    {

        for(int r = 0; r < Sequencer.RowAmount; r++)
        {
            for(int c = 0; c < Sequencer.StepAmount; c++)
            {
                var beatIndex = Steps[(r * Sequencer.StepAmount) + c].transform.GetSiblingIndex() % Sequencer.StepAmount;

                if (c == ((Sequencer.CurrentStep - 1) % Sequencer.StepAmount))
                {
                    Steps[(r * Sequencer.StepAmount) + c].SetMat(Prefabs.Instance.Blue);
                }
                else
                {
                    Steps[(r * Sequencer.StepAmount) + c].SetMat(Prefabs.Instance.White);
                }
            }
        }
    }
}

public enum DisplayType
{
    Linear,
    Circular
}
