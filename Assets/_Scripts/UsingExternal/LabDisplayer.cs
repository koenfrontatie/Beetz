using Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class LabDisplayer : MonoBehaviour
{
    [SerializeField] SampleObjectCollection _templateObjects;

    [SerializeField] Transform _scalerParent;

    [SerializeField] private TMPro.TextMeshProUGUI _nameText;


    public SampleObject SelectedTemplate;

    public Deformer DeformerGroup;

    private void OnEnable()
    {
        Metronome.NewBeat += OnNewBeat;
        Events.SetSelectedGuid += OnSetSelectedGuid;
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if(state == GameState.Biolab) OnSetSelectedGuid(AssetBuilder.Instance.SelectedGuid);
    }

    private void OnDisable()
    {
        Metronome.NewBeat -= OnNewBeat;
        Events.SetSelectedGuid -= OnSetSelectedGuid;
        GameManager.StateChanged -= OnStateChanged;

    }

    private async void OnSetSelectedGuid(string guid)
    {
        _scalerParent.DestroyChildren();

        if (string.IsNullOrEmpty(AssetBuilder.Instance.SelectedGuid)) return;
        var so = await AssetBuilder.Instance.GetSampleObject(guid);

        so.transform.SetParent(_scalerParent);

        so.transform.localScale = Vector3.one;
        //so.transform.localPosition = Vector3.zero;
        so.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(18f, 0, 0));
        so.transform.position = _scalerParent.position;


        //so.transform.gameObject.layer = LayerMask.NameToLayer("UI");
        //foreach (Transform child in so.transform)
        //{
        //    child.gameObject.layer = LayerMask.NameToLayer("UI");
        //    foreach (Transform child2 in child)
        //    {
        //        child2.gameObject.layer = LayerMask.NameToLayer("UI");
        //    }
        //}

        //_nameText.text = so.SampleData.Name;

        SelectedTemplate = so;

        AddDeformables(so.transform);

        Events.OnScaleBounce?.Invoke(so.gameObject);
        //so.gameObject.transform.SetParent(_scalerParent);
        //if (AssetBuilder.Instance.SelectedGuid == guid)
        //{
        //    SampleObject so = AssetBuilder.Instance.SelectedSampleObject;
        //    if (so != null)
        //    {
        //        if (so.Preview != null)
        //        {
        //            so.Preview.transform.SetParent(_scalerParent);
        //            so.Preview.transform.localPosition = Vector3.zero;
        //            so.Preview.transform.localRotation = Quaternion.identity;
        //            so.Preview.transform.localScale = Vector3.one;
        //        }
        //    }
        //}
    }

    private void OnNewBeat()
    {
        if(GameManager.Instance.State != GameState.Biolab) return;
        Events.OnScaleBounce?.Invoke(SelectedTemplate.gameObject);
        //string s = $"i {(int)(SelectedTemplate.SampleData.Template + 1)} 0 6";
        Events.LoadPlayGuid?.Invoke(SelectedTemplate.SampleData.ID);
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
