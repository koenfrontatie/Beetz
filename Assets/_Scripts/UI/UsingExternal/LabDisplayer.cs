using Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;
using System.Threading.Tasks;

public class LabDisplayer : MonoBehaviour
{
    [SerializeField] SampleObjectCollection _templateObjects;

    [SerializeField] Transform _scalerParent;

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;


    public SampleObject SelectedObject;

    public Deformer DeformerGroup;

    private void OnEnable()
    {
        Metronome.NewBeat += OnNewBeat;
        FileManager.NewSampleSelected += OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if(state == GameState.Biolab) OnSetSelectedGuid(FileManager.Instance.SelectedSampleGuid);
    }

    private void OnDisable()
    {
        Metronome.NewBeat -= OnNewBeat;
        FileManager.NewSampleSelected -= OnSetSelectedGuid;
        GameManager.StateChanged -= OnStateChanged;

    }

    private async void OnSetSelectedGuid(string guid)
    {
        _scalerParent.DestroyChildren();

        if (string.IsNullOrEmpty(FileManager.Instance.SelectedSampleGuid)) return;

        //Debug.Log("lab on selected");
        
        SelectedObject = await AssetBuilder.Instance.GetSampleObject(guid);

        SelectedObject.transform.SetParent(_scalerParent);

        SelectedObject.transform.localScale = Vector3.one;
        //SelectedObject.transform.localPosition = Vector3.zero;
        SelectedObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(18f, 0, 0));
        
        SelectedObject.transform.GetChild(0).transform.localPosition = Vector3.zero;
        //SelectedObject.transform.GetChild(0).transform.localRotation = Quaternion.identity;

        //SelectedObject.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        //foreach (Transform child in SelectedObject.transform)
        //{
        //    child.gameObject.layer = LayerMask.NameToLayer("UI");
        //    foreach (Transform child2 in child)
        //    {
        //        child2.gameObject.layer = LayerMask.NameToLayer("UI");
        //    }
        //}

        //_nameText.text = SelectedObject.SampleData.Name;

        //SelectedTemplate = SelectedObject;

        AddDeformables(SelectedObject.transform);

        //Events.OnScaleBounce?.Invoke(SelectedObject.gameObject);
        //SelectedObject.gameObject.transform.SetParent(_scalerParent);
        //if (FileManager.Instance.SelectedSampleGuid == guid)
        //{
        //    SampleObject SelectedObject = AssetBuilder.Instance.SelectedSampleObject;
        //    if (SelectedObject != null)
        //    {
        //        if (SelectedObject.Preview != null)
        //        {
        //            SelectedObject.Preview.transform.SetParent(_scalerParent);
        //            SelectedObject.Preview.transform.localPosition = Vector3.zero;
        //            SelectedObject.Preview.transform.localRotation = Quaternion.identity;
        //            SelectedObject.Preview.transform.localScale = Vector3.one;
        //        }
        //    }
        //}
    }

    private void OnNewBeat()
    {
        if(GameManager.Instance.State != GameState.Biolab) return;
        Events.OnScaleBounce?.Invoke(SelectedObject.gameObject);
        //string s = $"i {(int)(SelectedTemplate.SampleData.Template + 1)} 0 6";
        //Events.LoadPlayGuid?.Invoke(SelectedTemplate.SampleData.ID);
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        var current = SelectedTemplate.SampleData.Template;
    //        var choice = 0;

    //        if(current == 0 )
    //        {
    //            choice = _templateObjects.Collection[_templateObjects.Collection.Count - 1].SampleData.Template;
    //        } else
    //        {
    //            choice = current - 1;
    //        }

    //        SelectNewTemplate(choice);
    //    }

    //    if(Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        var current = SelectedTemplate.SampleData.Template;
    //        var choice = 0;

    //        if (current == _templateObjects.Collection[_templateObjects.Collection.Count - 1].SampleData.Template)
    //        {
    //            choice = 0;
    //        }
    //        else
    //        {
    //            choice = current + 1;
    //        }

    //        SelectNewTemplate(choice);
    //    }
    //}

    //private void Start()
    //{
    //    SelectNewTemplate(0);
    //}

    //void SelectNewTemplate(int template)
    //{
    //    if (SelectedTemplate != null) Destroy(SelectedTemplate.gameObject);

    //    var obj = Instantiate(_templateObjects.Collection[template], transform);

    //    SelectedTemplate = obj;

    //    AddDeformables(obj.transform);
    //}

    void AddDeformables(Transform sampleObject)
    {
        foreach(Transform child in sampleObject.GetChild(0))
        {
            //if(child.)
            if(child.TryGetComponent<MeshRenderer>(out var rend)) {

                Deformable deform = child.gameObject.AddComponent<Deformable>();
                deform.AddDeformer(DeformerGroup);
            }
        }
    }
}
