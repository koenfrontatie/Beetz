
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateState(GameState state)
    {
        State = state;

        switch(State)
        {
            case GameState.Viewing:
                
                break;
            case GameState.Placing:
                
            break;
        }

        Events.OnGameStateChanged?.Invoke(State);
    }

    public void UpdateState(int state)
    {
        State = (GameState)state;

        switch (State)
        {
            case GameState.Viewing:

                break;
            case GameState.Placing:

                break;
        }

        Events.OnGameStateChanged?.Invoke(State);
    }
}

public enum GameState
{
    Viewing,
    Placing
}
