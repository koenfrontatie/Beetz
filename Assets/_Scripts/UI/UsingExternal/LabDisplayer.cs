using Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using System.Threading.Tasks;
using System.IO;


public class LabDisplayer : MonoBehaviour
{
    [SerializeField]
    private LibraryController _libraryController;
    [SerializeField] 
    SampleObjectCollection _templateObjects;
    [SerializeField] 
    Transform _objectParent;
    [SerializeField] 
    private TMPro.TMP_InputField _nameInput;
    [SerializeField] 
    private SampleObject _selectedObject;

    [SerializeField]
    private DSPController _dspController;
    [SerializeField]
    private EffectSelector _effectSelector;

    [SerializeField] 
    private Deformer _deformerGroup;
    [SerializeField]
    private Deformable _currentDeformable;
    [SerializeField]
    private SquashAndStretchDeformer _squashAndStretchDeformer;
    [SerializeField]
    private SimplexNoiseDeformer _simplexNoiseDeformer;
    [SerializeField]
    private TwistDeformer _twistDeformer;

    public List<EffectValuePair> ActiveEffects;
    
    private bool _dataUpdated, saveOnSliderChange;

    

    private void OnEnable()
    {
        //Metronome.NewBeat += OnNewBeat;
        //FileManager.NewSampleSelected += OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;
    }
    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Biolab)
        {
            //FileManager.NewSampleSelected += OnSetSelectedGuid;
            OnSetSelectedGuid(FileManager.Instance.SelectedSampleGuid);
           
            Metronome.NewBeat += OnNewBeat;
        } else
        {
           
            Metronome.NewBeat -= OnNewBeat;
            //FileManager.NewSampleSelected -= OnSetSelectedGuid;
            //if(_selectedObject != null) Destroy(_selectedObject.gameObject);    
        }
    }

    private async void OnSetSelectedGuid(string guid) 
    {
        if(GameManager.Instance.State != GameState.Biolab) return;

        _dataUpdated = false;

        saveOnSliderChange = false;
        
        _effectSelector.ResetSliders();
        _dspController.ResetDefaultValues();

        saveOnSliderChange = true; // lol
        
        Debug.Log("spawning lab object");
        
        _objectParent.DestroyChildren();

        if(_selectedObject != null) Destroy(_selectedObject.gameObject);

        if (string.IsNullOrEmpty(FileManager.Instance.SelectedSampleGuid)) return;
        
        SampleData data = await AssetBuilder.Instance.GetSampleData(guid);

        SampleObject loadedObject = await AssetBuilder.Instance.GetSampleObject(data.Template.ToString());
        
        _selectedObject = loadedObject;

        _selectedObject.transform.SetParent(_objectParent);

        _selectedObject.transform.localScale = Vector3.one;
        
        _selectedObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _selectedObject.SampleData = data;

        AddDeformables(_selectedObject.gameObject);

        // get existent data

        //_dspController.ActiveEffects = _selectedObject.SampleData.Effects;
        
        _nameInput.SetTextWithoutNotify(_selectedObject.SampleData.Name);

        //Debug.Log(_selectedObject.SampleData.Name);
        //_nameText.text = _selectedObject.SampleData.Name.ToString();
        
        if (_selectedObject.SampleData.Effects != null && _selectedObject.SampleData.Effects.Count > 0)
        {
            List<EffectValuePair> effectsCopy = new List<EffectValuePair>(_selectedObject.SampleData.Effects);

            foreach (var effect in effectsCopy)
            {
                //_effectSelector.SetSliderValue(effect.Effect, effect.Value);
                _effectSelector.SetSliderValue(effect.Effect, effect.Value);
            }
        }


        _dataUpdated = false;
    }

    private void OnNewBeat()
    {
        if(GameManager.Instance.State != GameState.Biolab) return;
        if(_selectedObject == null) return;
        Events.OnScaleBounce?.Invoke(_selectedObject.gameObject);
    }

    void AddDeformables(GameObject sampleObject)
    {
        foreach(Transform child in sampleObject.transform.GetChild(0))
        {
            //if(child.)
            if(child.TryGetComponent<MeshRenderer>(out var rend)) {

                _currentDeformable = child.gameObject.AddComponent<Deformable>();
                _currentDeformable.AddDeformer(_deformerGroup);
            }
        }
    }
    public void OnPitchSliderChange(float value)
    {
        var pitch = Mathf.Clamp(value, 0, 1);
        
        _dspController.SetPitch(pitch);
        _squashAndStretchDeformer.Factor = pitch.Remap(0, 1, -.8f, .8f);
        _squashAndStretchDeformer.Curvature = pitch.Remap(0, 1, 0f, -20f);

        _dataUpdated = true;
        //UpdateSampleData();
    }

    public void OnDistortionSliderChange(float value)
    {
        //var dist = Mathf.Abs(value.Remap(.5f, 1f, 0, 1));

        _dspController.SetDistortion(value);
        //_dspController.SetDelay(value.Remap(0, 1f, 0f, 1f));
        _simplexNoiseDeformer.FrequencyScalar = value.Remap(.5f, 1, 0f, 7.5f);


        _dataUpdated = true;
    }

    public void OnDelaySliderChange(float value)
    {
        //var delay = Mathf.Abs(value.Remap(.5f, 1f, 0, 1));

        //_dspController.SetLiveDistortion(DistortionValue);
        _dspController.SetDelay(value);
        _simplexNoiseDeformer.FrequencyScalar = value.Remap(.5f, 1f, 0f, 7.5f); // should be delayeffect visual
        _dataUpdated = true;
    }

    public void OnChorusSliderChange(float value)
    {
        //_dspController.SetLiveDistortion(DistortionValue);
        _dspController.SetChorus(value);
        _twistDeformer.Factor = value.Remap(.5f, 1f, 0, 1);
        _dataUpdated = true;
    }

    public void OnReverbSliderChange(float value)
    {
        //_dspController.SetLiveDistortion(DistortionValue);
        _dspController.SetReverb(value);
        _twistDeformer.Factor = value.Remap(.5f, 1f, 0, 1);
        _dataUpdated = true;
    }

    //bool SampleDataUpdated()
    //{
    //    bool hasUpdated = false;

    //    if (_loadedSampleData.Effects != _dspController.ActiveEffects)
    //    {
    //        //_loadedSampleData.Effects = _dspController.ActiveEffects;

    //        hasUpdated = true;
    //    }

    //    if (!string.Equals(_loadedSampleData.Name, _nameInput.text))
    //    {
    //        //_loadedSampleData.Name = _nameInput.text;

    //        hasUpdated = true;
    //    }

    //    return hasUpdated;
    //}

    public async void SaveVariableData()
    {
        if(!saveOnSliderChange) return;

        Debug.Log("Saving variable data...");

        if(NameUpdated())
            _dataUpdated = true;
        
        _selectedObject.SampleData.Effects = _dspController.ActiveEffects;
        
        if (_dataUpdated)
        {
            _libraryController.ClearFromDictionary(FileManager.Instance.SelectedSampleGuid);

            Vector3[] vertices = _currentDeformable.GetCurrentMesh().vertices;
            var meshPath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "verts.json");
            var samplePath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "SampleData.json");
            
            AssetBuilder.Instance.AddToDictionary(FileManager.Instance.SelectedSampleGuid, vertices);

            await Task.Run(() =>
            {
                SaveLoader.Instance.SaveData<Vector3[]>(meshPath, vertices);
                SaveLoader.Instance.SaveData<SampleData>(samplePath, _selectedObject.SampleData);
            });

            _libraryController.RefreshInfoTiles();

            Debug.Log("Variable data saved and library controller refreshed.");

            return;
        }

        Debug.Log("No update to data, no saving needed.");

        //_effectSelector.ResetSliders(); // this should probably go somewhere else
    }

    public async void SaveDeformableMesh()
    {
        Vector3[] vertices = _currentDeformable.GetCurrentMesh().vertices;

        AssetBuilder.Instance.AddToDictionary(FileManager.Instance.SelectedSampleGuid, vertices);

        await Task.Run(() =>
        {
            var meshPath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "verts.json");
            SaveLoader.Instance.SaveData<Vector3[]>(meshPath, vertices);
            //AssetBuilder.Instance.dire
            //SaveLoader.Instance.SaveData<Mesh>(meshPath, mesh);

        });
    }


    public bool NameUpdated()
    {
        if (!string.Equals(_selectedObject.SampleData.Name, _nameInput.text))
        {

            _selectedObject.SampleData.Name = _nameInput.text;
            return true;
        }

        return false;
    }

    //public async void SaveSampleData()
    //{
    //    await Task.Run(() =>
    //    {
    //        var samplePath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "SampleData.json");
    //        SaveLoader.Instance.SaveData<SampleData>(samplePath, _selectedObject.SampleData);
    //    });

    //    //_libraryController.RefreshInfoTiles();

    //}
}
