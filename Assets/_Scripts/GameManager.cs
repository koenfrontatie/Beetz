using System;
using UnityEngine;
//using FileManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static Action<GameState> StateChanged;

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
    private void OnEnable()
    {
        DataStorage.ProjectDataSet += (data) => UpdateState(GameState.Gameplay);
    }
    private void OnDisable()
    {
        DataStorage.ProjectDataSet -= (data) => UpdateState(GameState.Gameplay);
    }
    private void Start()
    {
        UpdateState(GameState.Init);
    }

    public void UpdateState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Init:
                break;
            case GameState.Menu:
                break;
            case GameState.Gameplay:

                break;
            case GameState.Library:

                break;
            case GameState.Biolab:

                break;
            case GameState.ProjectSelection:

                break;
        }

        StateChanged?.Invoke(state);
    }
    public void OpenMenu() // for button
    {
        if (State != GameState.Menu)
        {
            UpdateState(GameState.Menu);
        }
        else
        {
            UpdateState(GameState.Gameplay);
        }
    }
    public void OpenLibrary() // for button
    {
        if (State != GameState.Library)
        {
            UpdateState(GameState.Library);
        }
        else
        {
            UpdateState(GameState.Gameplay);
        }
    }
    public void OpenBioLab() // for button
    {
        if (State != GameState.Biolab)
        {
            UpdateState(GameState.Biolab);
        }
        else
        {
            UpdateState(GameState.Library);
        }
    }

    public void OpenProjectSelection() // for button
    {
        if (State != GameState.ProjectSelection)
        {
            UpdateState(GameState.ProjectSelection);
        }
        else
        {
            UpdateState(GameState.Menu);
        }
    }

    public void ToggleBetweenStates(GameState state1, GameState state2)
    {
        if (State != state1)
        {
            UpdateState(state1);
        }
        else
        {
            UpdateState(state2);
        }
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
}

[Serializable]
public enum GameState
{
    Init,
    Menu,
    Gameplay,
    Library,
    Biolab,
    ProjectSelection,
    CircularEdit
}
