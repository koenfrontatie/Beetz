using Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabDisplayer : MonoBehaviour
{
    [SerializeField] SampleObjectsSO _templateObjects;

    public SampleObject SelectedTemplate;

    public Deformer DeformerGroup;

    private void OnEnable()
    {
        Metronome.NewBeat += OnNewBeat;
    }
    private void OnDisable()
    {
        Metronome.NewBeat -= OnNewBeat;
    }
    private void OnNewBeat()
    {
        Events.OnScaleBounce?.Invoke(SelectedTemplate.gameObject);
        string s = $"i {(int)(SelectedTemplate.SampleData.Template + 1)} 0 6";
        Events.OnScoreEvent?.Invoke(s);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var current = SelectedTemplate.SampleData.Template;
            var choice = 0;
            
            if(current == 0 )
            {
                choice = _templateObjects.Collection[_templateObjects.Collection.Count - 1].SampleData.Template;
            } else
            {
                choice = current - 1;
            }
            
            SelectNewTemplate(choice);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            var current = SelectedTemplate.SampleData.Template;
            var choice = 0;

            if (current == _templateObjects.Collection[_templateObjects.Collection.Count - 1].SampleData.Template)
            {
                choice = 0;
            }
            else
            {
                choice = current + 1;
            }

            SelectNewTemplate(choice);
        }
    }

    private void Start()
    {
        SelectNewTemplate(0);
    }

    void SelectNewTemplate(int template)
    {
        if (SelectedTemplate != null) Destroy(SelectedTemplate.gameObject);

        var obj = Instantiate(_templateObjects.Collection[template], transform);

        SelectedTemplate = obj;

        AddDeformables(obj.transform);
    }

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
