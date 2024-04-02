using log4net.Util;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearDisplayer : MonoBehaviour, IDisplayer
{
    public List<Step> Steps = new List<Step>();
    
    private Sequencer _sequencer;
    private PlaylistPlayback _playlistPlayback;

    void OnEnable()
    {
        Metronome.OnPlayPause += (b) => UpdateStepColors();
        Metronome.OnStep += UpdateStepColors;
        Metronome.OnResetMetronome += UpdateStepColors;
    }

    public void SpawnSteps()
    {
        _sequencer = GetComponent<Sequencer>();
        _playlistPlayback = GetComponent<PlaylistPlayback>();

        float x = transform.localPosition.x;

        for (int r = 0; r < _sequencer.RowAmount; r++)
        {
            for (int i = 0; i < _sequencer.StepAmount; i++)
            {
                Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x + Config.CellSize * i, 0f, transform.localPosition.z - r * Config.CellSize), Quaternion.identity, transform.GetChild(0));
                step.BeatIndex = i % (int)_sequencer.SequencerData.Dimensions.x;
                Steps.Add(step);
            }
        }

        Events.OnStepsPlaced(_sequencer);
    }
    public void UpdateStepColors()
    {
        if (!_sequencer._isLooping) //updates step materials based on PlaylistPlayback component
        {
            for (int r = 0; r < _sequencer.RowAmount; r++)
            {
                for (int c = 0; c < _sequencer.StepAmount; c++)
                {
                    var beatIndex = Steps[(r * _sequencer.StepAmount) + c].transform.GetSiblingIndex() % _sequencer.StepAmount;

                    if (c == ((_playlistPlayback.PlaylistStep) % (_sequencer.StepAmount)) && _playlistPlayback.PlaylistStep != -1)
                    {
                        Steps[(r * _sequencer.StepAmount) + c].SetColor(Prefabs.Instance.ActiveStep);
                    }
                    else
                    {
                        Steps[(r * _sequencer.StepAmount) + c].SetColor(Prefabs.Instance.PassiveStep);
                    }
                }
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
                    Steps[(r * _sequencer.StepAmount) + c].SetColor(Prefabs.Instance.ActiveStep);
                }
                else
                {
                    Steps[(r * _sequencer.StepAmount) + c].SetColor(Prefabs.Instance.PassiveStep);
                }
            }
        }
    }

    void OnDisable()
    {
        Metronome.OnPlayPause -= (b) => UpdateStepColors();
        Metronome.OnStep -= UpdateStepColors;
        Metronome.OnResetMetronome -= UpdateStepColors;
    }
}
