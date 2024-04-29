using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _settingsMenu, _mainScene, _biolabScene, _library, _toolbar, _libToolbar;

    //[SerializeField] GridLayoutGroup _toolbarLayout;

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
    }

    public void OnStateChanged(GameState state)
    {
        ToggleAllCanvases(false);

        switch (state)
        {
            case GameState.Menu:
                _settingsMenu.ToggleCanvasGroup(true);
            break;
            case GameState.Gameplay:
                _mainScene.ToggleCanvasGroup(true);
                _toolbar.ToggleCanvasGroup(true);
                _libToolbar.gameObject.SetActive(false);
                _toolbar.gameObject.SetActive(true);
                _toolbar.GetComponent<Toolbar>().OnRefreshToolbar();

                break;
            case GameState.Library:
                _library.ToggleCanvasGroup(true);
                _libToolbar.ToggleCanvasGroup(true);
                _toolbar.gameObject.SetActive(false);
                _libToolbar.gameObject.SetActive(true);
                _libToolbar.GetComponent<Toolbar>().OnRefreshToolbar();
                _library.GetComponent<LibraryController>().RefreshInfoTiles();  
                break;
        }
    }

    void ToggleAllCanvases(bool b)
    {
        _settingsMenu.ToggleCanvasGroup(b);
        //_settingsMenu.gameObject.SetActive(true);

        _mainScene.ToggleCanvasGroup(b);
        //_biolabScene.ToggleCanvasGroup(b);
        _library.ToggleCanvasGroup(b);

        _toolbar.ToggleCanvasGroup(b);

        _libToolbar.ToggleCanvasGroup(b);
    }

    //void ToggleToolbar(bool b)
    //{
    //    _libraryButton.gameObject.SetActive(!b);
    //    _upDownButton.gameObject.SetActive(!b);

    //    if(!b)
    //    {
    //        _toolbarLayout.constraintCount = 6;
    //    } else
    //    {
    //        _toolbarLayout.constraintCount = 15;
    //    }
    //}

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }
}

