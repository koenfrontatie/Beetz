using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularDisplayer : MonoBehaviour, IDisplayer
{
    public List<Step> Steps = new List<Step>();

    [SerializeField] private float _rowSpacing = .5f;

    [SerializeField] private float _innerOffset = 1f;

    [SerializeField] private Transform _circularSoil;

    private float _radius;
    public float GetRadius () => _radius;

    private Sequencer _sequencer;
    public Step GetStepFromIndex(int index)
    {
        return Steps[index];
    }
    public void SpawnSteps()
    {
        _sequencer = GetComponent<Sequencer>();

        float angleStep = (360 / (float)_sequencer.StepAmount);
        float stepAdjustment = _rowSpacing * (_sequencer.StepAmount / 16f); // if long pattern inner radius is bigger
        float variableInnerOffset = stepAdjustment + Mathf.Clamp(_rowSpacing * 3.3f - _rowSpacing * _sequencer.RowAmount, 0, 5f); // bigger radius for small rowcount
        //float variableIncrement = Mathf.Clamp(3f / _sequencer.RowAmount, 1, 5f);

        for (int j = 0; j < _sequencer.RowAmount; j++)
        {
            for (int i = 0; i < _sequencer.StepAmount; i++)
            {
                float angle = -angleStep * i;
                float radians = Mathf.Deg2Rad * -angle;
                
                var rowIncrement = _rowSpacing * (j + 1);
                
                
                float x = Mathf.Sin(radians) * (rowIncrement + variableInnerOffset + _innerOffset);
                float z = Mathf.Cos(radians) * (rowIncrement + variableInnerOffset + _innerOffset);
                
                Vector3 spawnPos = new Vector3(x, 0, z);
                Quaternion spawnRot = Quaternion.Euler(0f, -angle, 0f);
                Step step = Instantiate(Prefabs.Instance.Step, transform.GetChild(0));
                step.transform.localPosition = spawnPos;
                step.transform.localRotation = spawnRot;
                step.BeatIndex = i % (int)_sequencer.SequencerData.Dimensions.x;

                Steps.Add(step);
            }
        }

        var inc = _rowSpacing * (_sequencer.RowAmount + 1) * 2;
        var offset = variableInnerOffset * 2 + _innerOffset * 2;
        
        _circularSoil.localScale = new Vector3(inc + offset, inc + offset, 1f);

        _radius = _rowSpacing * (_sequencer.RowAmount + 1) * Config.CellSize;

        Events.OnStepsPlaced(gameObject);
    }
    public void UpdateStepColors()
    {
        if (_sequencer.CurrentStep == -1)
        {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].SetColor(Config.PassiveStep);
            }

            return;
        }

        for (int r = 0; r < _sequencer.RowAmount; r++)    //updates step materials based on sequencer component
        {
            for (int c = 0; c < _sequencer.StepAmount; c++)
            {
                var beatIndex = Steps[(r * _sequencer.StepAmount) + c].transform.GetSiblingIndex() % _sequencer.StepAmount;

                if (c == ((_sequencer.CurrentStep - 1) % _sequencer.StepAmount))
                {
                    Steps[(r * _sequencer.StepAmount) + c].SetColor(Config.ActiveStep);
                }
                else
                {
                    Steps[(r * _sequencer.StepAmount) + c].SetColor(Config.PassiveStep);
                }
            }
        }
    }
}
