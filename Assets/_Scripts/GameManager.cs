using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        //Metronome.OnStep += StepLog;
        //Metronome.OnBeat += BeatLog;
    }
    public void UpdateState(GameState state)
    {
        State = state;

        switch(State)
        {
            case GameState.Viewing:
                
                break;
            case GameState.Patching:
                
            break;
        }

        Events.OnGameStateChanged?.Invoke(State);
    }

   
    void StepLog() {
        Debug.Log("Step");
    }
    void BeatLog()
    {
        Debug.Log("Beat");
    }
    public void UpdateState(int state)
    {
        State = (GameState)state;

        switch (State)
        {
            case GameState.Viewing:

                break;
            case GameState.Patching:

                break;
        }

        Events.OnGameStateChanged?.Invoke(State);
    }
}

public enum GameState
{
    Viewing,
    Patching
}
