using Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.UI;
using Un4seen.Bass;

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
    private Deformer _deformerGroup;
    [SerializeField]
    private Deformable _currentDeformable;
    [SerializeField]
    private SquashAndStretchDeformer _squashAndStretchDeformer;
    [SerializeField]
    private SimplexNoiseDeformer _simplexNoiseDeformer;

    public float PitchValue;
    public float ReverbValue;
    public float DistortionValue;
    public float DelayValue;

    public List<EffectValuePair> ActiveEffects;

    private void OnEnable()
    {
        Metronome.NewBeat += OnNewBeat;
        FileManager.NewSampleSelected += OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Biolab)
        {
            Metronome.NewBeat += OnNewBeat;
            FileManager.NewSampleSelected += OnSetSelectedGuid;
           
            OnSetSelectedGuid(FileManager.Instance.SelectedSampleGuid);
        } else
        {
           
            Metronome.NewBeat -= OnNewBeat;
            FileManager.NewSampleSelected -= OnSetSelectedGuid;
            if(_selectedObject != null) Destroy(_selectedObject.gameObject);    
        }
    }

    private async void OnSetSelectedGuid(string guid) 
    {
        if(GameManager.Instance.State != GameState.Biolab) return;
        
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

        ActiveEffects = _selectedObject.SampleData.Effects;
        
        _nameInput.SetTextWithoutNotify(_selectedObject.SampleData.Name);

        //Debug.Log(_selectedObject.SampleData.Name);
        //_nameText.text = _selectedObject.SampleData.Name.ToString();
        if (ActiveEffects != null && ActiveEffects.Count > 0)
        {
            foreach (var effect in ActiveEffects)
            {
                switch (effect.Effect)
                {
                    case EffectType.Pitch:
                        UpdatePitchValue(effect.Value);
                        break;
                    case EffectType.Distortion:
                        UpdateDistortionValue(effect.Value);
                        break;
                }
            }
        }  
    }

    private void OnNewBeat()
    {
        if(GameManager.Instance.State != GameState.Biolab) return;
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
    public void UpdatePitchValue(float value)
    {
        PitchValue = Mathf.Clamp(value, -1, 1);

        //_dspController.SetLivePitch(PitchValue.Remap(-1, 1, .5f, 2f));
        _dspController.SetPitch(PitchValue.Remap(-1, 1, .5f, 2f));

        _squashAndStretchDeformer.Factor = PitchValue.Remap(-1, 1, -.8f, .8f);
        _squashAndStretchDeformer.Curvature = PitchValue.Remap(-1, 1, 0f, -20f);

        bool closeToZero = Mathf.Round(PitchValue * 10.0f) * 0.1f == 0;

        var pitchEffect = ActiveEffects.Find(e => e.Effect == EffectType.Pitch);

        if (pitchEffect == null && !closeToZero)
        {
            ActiveEffects.Add(new EffectValuePair(EffectType.Pitch, PitchValue));
        }
        else if (pitchEffect != null && closeToZero)
        {
            ActiveEffects.Remove(pitchEffect);
        }
        else if (pitchEffect != null)
        {
            pitchEffect.Value = PitchValue;
        }

        UpdateSampleData();
    }

    public void UpdateDistortionValue(float value)
    {
        DistortionValue = Mathf.Abs(Mathf.Clamp(value, -1, 1));

        //_dspController.SetLiveDistortion(DistortionValue);
        _dspController.SetReverb(DistortionValue.Remap(0, 1, 0f, 1f));
        _simplexNoiseDeformer.FrequencyScalar = DistortionValue.Remap(0, 1, 0f, 7.5f);

        bool closeToZero = Mathf.Round(DistortionValue * 10.0f) * 0.1f == 0;

        var distortionEffect = ActiveEffects.Find(e => e.Effect == EffectType.Distortion);

        if (distortionEffect == null && !closeToZero)
        {
            ActiveEffects.Add(new EffectValuePair(EffectType.Distortion, DistortionValue));
        }
        else if (distortionEffect != null && closeToZero)
        {
            ActiveEffects.Remove(distortionEffect);
        }
        else if (distortionEffect != null)
        {
            distortionEffect.Value = DistortionValue;
        }

        UpdateSampleData();
    }

    void UpdateSampleData()
    {
        if (_selectedObject.SampleData.Effects != ActiveEffects)
        {
            _selectedObject.SampleData.Effects = ActiveEffects;
        }
    }

    public async void SaveVariableData()
    {
        Debug.Log("Saving variable data...");
        _libraryController.ClearFromDictionary(FileManager.Instance.SelectedSampleGuid);

        if (!string.Equals(_selectedObject.SampleData.Name, _nameInput.text))
        {
            _selectedObject.SampleData.Name = _nameInput.text;
        }



        Vector3[] vertices = _currentDeformable.GetCurrentMesh().vertices;
        var meshPath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "verts.json");
        var samplePath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "SampleData.json");
        AssetBuilder.Instance.AddToDictionary(FileManager.Instance.SelectedSampleGuid, vertices);


        await Task.Run(() =>
        {
            SaveLoader.Instance.SaveData<Vector3[]>(meshPath, vertices);
            SaveLoader.Instance.SaveData<SampleData>(samplePath, _selectedObject.SampleData);
            //_dspController.BakeLiveBassEffects(FileManager.Instance.SelectedSamplePath); 
        });


        _libraryController.RefreshInfoTiles();
        Debug.Log("Variable data saved and library controller refreshed.");
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

    public void CheckName()
    {
        if (!string.Equals(_selectedObject.SampleData.Name, _nameInput.text))
        {

            _selectedObject.SampleData.Name = _nameInput.text;
        }
    }   

    public async void SaveSampleData()
    {
        await Task.Run(() =>
        {
            var samplePath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, FileManager.Instance.SelectedSampleGuid, "SampleData.json");
            SaveLoader.Instance.SaveData<SampleData>(samplePath, _selectedObject.SampleData);
        });

        //_libraryController.RefreshInfoTiles();

    }
}
