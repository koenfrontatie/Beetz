//using System;
//using UnityEngine;

//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance;

//    public GameState State;

//    public static event Action<GameState> StateChanged;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(this);
//        }
//        else
//        {
//            Instance = this;
//        }
//    }

//    private void Start()
//    {
//        UpdateState(GameState.Menu);
//    }

//    public void UpdateState(GameState state)
//    {
//        State = state;

//        switch (State)
//        {
//            case GameState.Menu:

//                break;
//            case GameState.Gameplay:

//                break;
//            case GameState.Saving:
                
//                break;
//        }

//        StateChanged?.Invoke(State);
//    }
//}

//public enum GameState
//{
//    Menu,
//    Gameplay,
//    Saving
//}
