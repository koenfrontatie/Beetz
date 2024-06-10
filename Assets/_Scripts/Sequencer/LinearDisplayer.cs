using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LinearDisplayer : MonoBehaviour, IDisplayer
{
    public List<Step> Steps = new List<Step>();
    
    private Sequencer _sequencer;

    private void Awake()
    {
        _sequencer = GetComponent<Sequencer>();
    }
    public Step GetStepFromIndex(int index)
    {
        return Steps[index];
    }

    public async void SpawnSteps()
    {
        float x = transform.localPosition.x;
        if (Steps.Count > 0)
        {
            foreach (var step in Steps)
            {
                Destroy(step.gameObject);
            }
            Steps.Clear();
        }
        for (int r = 0; r < _sequencer.RowAmount; r++)
        {
            for (int i = 0; i < _sequencer.StepAmount; i++)
            {
                Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x + Config.CellSize * i, 0f, transform.localPosition.z - r * Config.CellSize), Quaternion.identity, transform.GetChild(0));
                step.BeatIndex = i % (int)_sequencer.SequencerData.Dimensions.x;
                Steps.Add(step);
            }
        }

        await Task.Delay(10);
        Events.OnStepsPlaced(gameObject);
    }
    public void UpdateStepColors()
    {
        if(_sequencer.CurrentStep == -1)
        {
            for(int i = 0; i < Steps.Count; i++)
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
