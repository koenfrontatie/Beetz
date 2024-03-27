using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menu;

    private void OnEnable()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    void OnStateChanged(GameState state)
    {
        _menu.ToggleCanvasGroup(false);

        Debug.Log("menu sees " + state.ToString());

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
        GameManager.Instance.StateChanged -= OnStateChanged;
    }
}

