using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    public static SampleManager Instance;

    public SampleObject SelectedSample;

    public List<string> LoadedLibrary = new List<string>();

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

    private void OnEnable()
    {
        Events.OnHotbarClicked += (i) => SelectedSample = Prefabs.Instance.BaseObjects[i];
    }
    private void Start()
    {
        LoadedLibrary.Clear();

        LoadedLibrary = SaveLoader.Instance.LoadProjectLibrary();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) SaveLoader.Instance.SaveListToBfc(LoadedLibrary);
        if (Input.GetKeyDown(KeyCode.L)) LoadedLibrary = SaveLoader.Instance.LoadProjectLibrary();
    }
    private void OnDisable()
    {
        Events.OnHotbarClicked -= (i) => SelectedSample = Prefabs.Instance.BaseObjects[i];
    }

    void SampleSelection(int i)
    {

    }

}
