using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    public static SampleManager Instance;

    public SampleObject SelectedSample;

    public List<string> Library = new List<string>(); 

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
        Library = SaveLoader.Instance.LoadProjectLibrary();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) SaveLoader.Instance.SaveListToBfc(Library);
        if (Input.GetKeyDown(KeyCode.L)) Library = SaveLoader.Instance.LoadProjectLibrary();
    }
    private void OnDisable()
    {
        Events.OnHotbarClicked -= (i) => SelectedSample = Prefabs.Instance.BaseObjects[i];
    }

    void LoadToolbar()
    {

    }

}
