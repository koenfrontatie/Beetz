using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menu;
    [SerializeField] private CanvasGroup _library;

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
                _menu.gameObject.SetActive(true);
            break;
            case GameState.Gameplay: 
                _menu.ToggleCanvasGroup(false);
                _menu.gameObject.SetActive(false);
                _library.ToggleCanvasGroup(false);
                _library.gameObject.SetActive(false);
                break;
            case GameState.Library:
                _library.ToggleCanvasGroup(true);
                _library.gameObject.SetActive(true);
                break;
        }
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }
}

