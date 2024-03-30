using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menu;

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
    }

    void OnStateChanged(GameState state)
    {
        _menu.ToggleCanvasGroup(false);

        switch (state)
        {
            case GameState.Menu:
                _menu.ToggleCanvasGroup(true);
            break;
            case GameState.Gameplay: 
                _menu.ToggleCanvasGroup(false); 
            break;
        }
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }
}

