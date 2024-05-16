using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _settingsMenu, _mainScene, _biolabScene, _library, _toolbar, _fileManager;
    [SerializeField] private Camera _main, _biolab;
    //[SerializeField] private 

    [SerializeField] private Button _editSample, _deleteSample, _newSample, _playPause;
    private Image _editFG, _editBG, _deleteFG, _deleteBG, _newFG, _newBG;
    //[SerializeField] GridLayoutGroup _toolbarLayout;

    private void Awake()
    {
        _editFG = _editSample.GetComponent<Image>();
        _editBG = _editSample.transform.parent.GetChild(0).GetComponent<Image>();
        _deleteFG = _deleteSample.GetComponent<Image>();
        _deleteBG = _deleteSample.transform.parent.GetChild(0).GetComponent<Image>();
        _deleteFG = _deleteSample.GetComponent<Image>();
        _deleteBG = _deleteSample.transform.parent.GetChild(0).GetComponent<Image>();
        _newFG = _newSample.GetComponent<Image>();
        _newBG = _newSample.transform.parent.GetChild(0).GetComponent<Image>();
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
            _newSample.interactable = true;

            _editFG.CrossFadeAlpha(.1f, .1f, false);
            _editBG.CrossFadeAlpha(.1f, .1f, false);
            
            _newFG.CrossFadeAlpha(1f, .1f, true);

            _deleteFG.CrossFadeAlpha(.1f, .1f, false);
            _deleteBG.CrossFadeAlpha(.1f, .1f, false);

            _newBG.CrossFadeAlpha(1f, .1f, true);
        }
        else
        {
           
                _editSample.interactable = true;
                _deleteSample.interactable = true;
                _newSample.interactable = false;

                _editFG.CrossFadeAlpha(1f, .1f, false);
                _editBG.GetComponent<Image>().CrossFadeAlpha(1f, .1f, false);
                _deleteFG.CrossFadeAlpha(1f, .1f, false);
                _deleteSample.transform.parent.GetChild(0).GetComponent<Image>().CrossFadeAlpha(1f, .1f, false);
                
                _newFG.CrossFadeAlpha(.1f, .1f, true);

                _newBG.CrossFadeAlpha(.1f, .1f, true);

           
        }
    }

    public void OnStateChanged(GameState state)
    {
        ToggleAllCanvases(false);
        _main.enabled = true;
        _biolab.enabled = false;
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
                if (Metronome.Instance.Playing)
                {
                    _playPause.onClick.Invoke();
                }
                _library.ToggleCanvasGroup(true);
                FindObjectOfType<LibraryController>().RefreshInfoTiles();
                _toolbar.ToggleCanvasGroup(true);
                break;

            case GameState.Biolab:
                if (Metronome.Instance.Playing)
                {
                    _playPause.onClick.Invoke();
                }
                _main.enabled = false;
                _biolab.enabled = true;
                _biolabScene.ToggleCanvasGroup(true);
                //_toolbar.ToggleCanvasGroup(true);
                break;
        }
    }

    void ToggleAllCanvases(bool b)
    {
        _settingsMenu.ToggleCanvasGroup(b);
        //_settingsMenu.gameObject.SetActive(true);

        _mainScene.ToggleCanvasGroup(b);
        _biolabScene.ToggleCanvasGroup(b);
        _library.ToggleCanvasGroup(b);

        _toolbar.ToggleCanvasGroup(b);

        _fileManager.ToggleCanvasGroup(b);
    }

    public void ToggleFileManager()
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

