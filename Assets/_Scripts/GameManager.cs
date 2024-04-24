using System;
using UnityEngine;

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

    private void Start()
    {
        UpdateState(GameState.Gameplay);
        DataStorage.Instance.OpenNewProject();
    }

    public void UpdateState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Menu:

                break;
            case GameState.Gameplay:

                break;
            case GameState.Library:
                break;
            case GameState.Saving:

                break;
        }

        StateChanged?.Invoke(state);
    }
    public void OpenMenu()
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

    public void ExitApplication()
    {
        Application.Quit();
    }
}

public enum GameState
{
    Menu,
    Gameplay,
    Library,
    Saving
}
