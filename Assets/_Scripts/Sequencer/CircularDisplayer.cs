using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularDisplayer : MonoBehaviour, IDisplayer
{
    public List<Step> Steps = new List<Step>();
    [SerializeField] private bool _looping = false;

    [SerializeField] private float _rowSpacing = .3f;
    [SerializeField] float _centerOffset = 0.3f;

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

        float angleStep = (360 / (float)_sequencer.StepAmount);
        float totalSpacing = _rowSpacing * _sequencer.StepAmount * _sequencer.RowAmount; // Assuming _spacing is the desired spacing between steps
        float radiusStep = totalSpacing / (2 * Mathf.PI * _sequencer.RowAmount); // Calculate the step size for the radius

        for (int j = 0; j < _sequencer.RowAmount; j++)
        {
            float radius = radiusStep * (j + 1) + _centerOffset; // Apply the uniform offset to each row

            for (int i = 0; i < _sequencer.StepAmount; i++)
            {
                float angle = -angleStep * i;
                float radians = Mathf.Deg2Rad * -angle;
                float x = Mathf.Sin(radians) * radius; // Use the radius for positioning
                float z = Mathf.Cos(radians) * radius; // Use the radius for positioning
                Vector3 spawnPos = new Vector3(x, 0, z);
                Quaternion spawnRot = Quaternion.Euler(0f, -angle, 0f);
                Step step = Instantiate(Prefabs.Instance.Step, transform.GetChild(0));
                step.transform.localPosition = spawnPos;
                step.transform.localRotation = spawnRot;
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
