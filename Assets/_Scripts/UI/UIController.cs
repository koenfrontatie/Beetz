using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _settingsMenu, _mainScene, _biolabScene, _library, _toolbar, _fileManager;
    [SerializeField] private GameObject _libraryPreviewObject;
    
    [SerializeField] private Button _editSample, _deleteSample, _playPause;
    private Image _editFG, _editBG, _deleteFG, _deleteBG;
    //[SerializeField] GridLayoutGroup _toolbarLayout;

    private void Awake()
    {
        _editFG = _editSample.GetComponent<Image>();
        _editBG = _editSample.transform.parent.GetChild(0).GetComponent<Image>();
        _deleteFG = _deleteSample.GetComponent<Image>();
        _deleteBG = _deleteSample.transform.parent.GetChild(0).GetComponent<Image>();
    }
    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
        Events.BaseSampleSelected += OnBaseSampleSelected;
    }

    private void OnBaseSampleSelected(bool b)
    {
        if(b)
        {
            _editSample.interactable = false;
            _deleteSample.interactable = false;

           _editFG.CrossFadeAlpha(.1f, .1f, false);
            _editBG.GetComponent<Image>().CrossFadeAlpha(.1f, .1f, false);

            _deleteFG.CrossFadeAlpha(.1f, .1f, false);
           _deleteBG.GetComponent<Image>().CrossFadeAlpha(.1f, .1f, false);

        }
        else
        {
            if (_editSample.interactable == false)
            {
                _editSample.interactable = true;
                _deleteSample.interactable = true;

               _editFG.CrossFadeAlpha(1f, .1f, false);
                _editBG.GetComponent<Image>().CrossFadeAlpha(1f, .1f, false);
                _deleteFG.CrossFadeAlpha(1f, .1f, false);
                _deleteSample.transform.parent.GetChild(0).GetComponent<Image>().CrossFadeAlpha(1f, .1f, false);
            }
        }
    }

    public void OnStateChanged(GameState state)
    {
        ToggleAllCanvases(false);
        switch (state)
        {
            case GameState.Menu:
                if (Metronome.Instance.Playing)
                {
                    _playPause.onClick.Invoke();
                }
                _settingsMenu.ToggleCanvasGroup(true);
            break;
            case GameState.Gameplay:
                _mainScene.ToggleCanvasGroup(true);
                _toolbar.ToggleCanvasGroup(true);
                break;
            case GameState.Library:
                if(Metronome.Instance.Playing)
                {
                    _playPause.onClick.Invoke();
                }
                _library.ToggleCanvasGroup(true);
                _library.GetComponent<LibraryController>().RefreshInfoTiles();
                _libraryPreviewObject.SetActive(true);
                _toolbar.ToggleCanvasGroup(true);
                break;
        }
    }

    public void ToggleTimeTable()
    {
        ToggleAllCanvases(false);
    }

    void ToggleAllCanvases(bool b)
    {
        _settingsMenu.ToggleCanvasGroup(b);
        //_settingsMenu.gameObject.SetActive(true);

        _mainScene.ToggleCanvasGroup(b);
        //_biolabScene.ToggleCanvasGroup(b);
        _library.ToggleCanvasGroup(b);

        _toolbar.ToggleCanvasGroup(b);

        _fileManager.ToggleCanvasGroup(b);

        _libraryPreviewObject.SetActive(b);
    }

    void ToggleFileManager()
    {
        _fileManager.ToggleCanvasGroup();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleFileManager();
        }
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
        Events.BaseSampleSelected -= OnBaseSampleSelected;

    }
}

