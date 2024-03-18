using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public abstract class Displayer : MonoBehaviour
{
    public Sequencer Sequencer { get; private set; }
    private PlaylistPlayback _playlistPlayback;
    public List<Step> Steps = new List<Step>();
    private void OnEnable()
    {
        Metronome.OnStep += UpdateMaterials;
        Metronome.OnResetMetronome += UpdateMaterials;
        //Events.OnNewSongRange += UpdateMaterials;
        //Events.OnSequencerMoved += (Sequencer s, Vector2 v) => UpdateMaterials();
    }
    private void OnDisable()
    {
        Metronome.OnStep -= UpdateMaterials;
        Metronome.OnResetMetronome -= UpdateMaterials;
        //Events.OnNewSongRange -= UpdateMaterials;
        //Events.OnSequencerMoved -= (Sequencer s, Vector2 v) => UpdateMaterials();
    }
    void Start()
    {
        Sequencer = GetComponent<Sequencer>();
        transform.TryGetComponent<PlaylistPlayback>(out _playlistPlayback);
        PositionSteps();
        UpdateMaterials();

        Events.OnStepsPlaced?.Invoke(Sequencer);

    }

    abstract public void PositionSteps();
    //abstract public void ChangeSizing();
    public void UpdateMaterials()
    {
        if (GridController.Instance.PlaylistPlaybackEnabled) //updates step materials based on PlaylistPlayback component
        {
            for (int r = 0; r < Sequencer.RowAmount; r++)
            {
                for (int c = 0; c < Sequencer.StepAmount; c++)
                {
                    var beatIndex = Steps[(r * Sequencer.StepAmount) + c].transform.GetSiblingIndex() % Sequencer.StepAmount;

                    if (c == ((_playlistPlayback.PlaylistStep) % (Sequencer.StepAmount)) && _playlistPlayback.PlaylistStep != -1) 
                    {
                        Steps[(r * Sequencer.StepAmount) + c].SetColor(Prefabs.Instance.ActiveStep);
                    }
                    else
                    {
                        Steps[(r * Sequencer.StepAmount) + c].SetColor(Prefabs.Instance.PassiveStep);
                    }
                }
            }


            return;
        }
       
        for(int r = 0; r < Sequencer.RowAmount; r++)    //updates step materials based on sequencer component
        {
            for(int c = 0; c < Sequencer.StepAmount; c++)
            {
                var beatIndex = Steps[(r * Sequencer.StepAmount) + c].transform.GetSiblingIndex() % Sequencer.StepAmount;

                if (c == ((Sequencer.CurrentStep - 1) % Sequencer.StepAmount))
                {
                    Steps[(r * Sequencer.StepAmount) + c].SetColor(Config.ActiveStep);
                }
                else
                {
                    Steps[(r * Sequencer.StepAmount) + c].SetColor(Config.PassiveStep);
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
