using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SequencerDisplayer : MonoBehaviour
{
    [SerializeField] private float stepDist = .3f;
    private Sequencer sequencer;
    private List<Step> steps = new List<Step>();

    private void OnEnable()
    {
        Metronome.OnStep += UpdateMaterials;
        Metronome.OnResetMetronome += UpdateMaterials;
    }
    private void Disable()
    {
        Metronome.OnStep -= UpdateMaterials;
        Metronome.OnResetMetronome -= UpdateMaterials;
    }
    void Start()
    {
        sequencer = GetComponent<Sequencer>();
        PositionSteps();
    }

    void PositionSteps()
    {
        float width = stepDist * (sequencer.StepAmount - 1);
        float x = transform.localPosition.x;

        for (int i = 0; i < sequencer.StepAmount; i++)
        {
            Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x - (width / 2f), 0f, transform.localPosition.z), Quaternion.identity, transform);
            steps.Add(step);
            x += stepDist;
        }

        UpdateMaterials();
    }

    void UpdateMaterials()
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
