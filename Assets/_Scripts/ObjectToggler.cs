using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToggler : MonoBehaviour
{
    public List<MonoBehaviour> CameraComponents = new List<MonoBehaviour>();
    public List<Camera> Cameras = new List<Camera>();
    public GameObject _mainEnvironment, _biolabEnvironment;

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }

    public void OnStateChanged(GameState state)
    {
        _biolabEnvironment.SetActive(false);
        _mainEnvironment.SetActive(false);
        
        foreach(Camera cam in Cameras)
        {
            cam.enabled = false;
        }

        foreach(MonoBehaviour comp in CameraComponents)
        {
            comp.enabled = false;
        }

        switch (state)
        {
            case GameState.Gameplay:
                _mainEnvironment.SetActive(true);
                Cameras[0].enabled = true;
                foreach (MonoBehaviour comp in CameraComponents)
                {
                    comp.enabled = true;
                }

                break;
            case GameState.Biolab:
                _biolabEnvironment.SetActive(true);
                break;
            case GameState.Library:
                break;
        }
    }

}
