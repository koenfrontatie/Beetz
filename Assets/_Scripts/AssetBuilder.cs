
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AssetBuilder : MonoBehaviour
{
    public static AssetBuilder Instance;

    [Header("Loaded Custom Sample GUID's")]
    public IDCollection CustomSamples;

    [Header("Loaded Custom Sample Icons")]
    public List<Texture2D> CustomSampleIcons;

    [Header("Current Selection")]
    public string SelectedGuid;

    [Space(10)]
    [Header("Template Prefabs")]
    [SerializeField]
    private SampleObjectCollection _templateSampleObjects;
    [SerializeField]
    private TextureCollection _templateIcons;

    [SerializeField]
    private GameObject _toolbarItemTemplate;

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
        Events.SampleSelected += (so) => SelectedGuid = so.SampleData.ID;
        //Events.ProjectDataLoaded += (data) => SearchForCustomSamples();
    }

    //private void Start()
    //{
    //    SearchForCustomSamples();
    //}
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Backspace))
    //    {
    //        Debug.Log("Backspace");
    //        FindCustomTextures();
    //    }
    //}
    
    public async void SearchForCustomSamples()
    {
        CustomSamples = await SaveLoader.Instance.GetCustomSampleCollection();
        //DataStorage.Instance.Cu
        FindCustomTextures();
    }

    public async void FindCustomTextures()
    {
        for (int i = 0; i < CustomSamples.IDC.Count; i++)
        {
            CustomSampleIcons.Insert(i, await SaveLoader.Instance.GetIconFromGuid(CustomSamples.IDC[i]));
        }
    }

    public SampleObject GetSampleObject(string guid)
    {
        if (guid.Length < 3) // if template
        {
            return _templateSampleObjects.Collection[int.Parse(guid)];
        } else
        {
            throw new NotImplementedException();
        }
    }

    private void OnDisable()
    {
        Events.SampleSelected -= (so) => SelectedGuid = so.SampleData.ID;

    }

    public GameObject GetToolbarItem(string guid)
    {
        GameObject item = Instantiate(_toolbarItemTemplate);

        var image = item.GetComponent<RawImage>();
        var so = item.GetComponent<SampleObject>();

        // if template

        if (guid.Length < 3)
        {
            if (guid == "-1")
            {
                image.texture = null;
                image.color = new Color(1, 1, 1, 0);
                return item;
            }

            var template = int.Parse(guid);

            image.texture = _templateIcons.Collection[template];

            so.SampleData = _templateSampleObjects.Collection[template].SampleData;

            return item;
        }

        // to do ---- load non templates

        image.texture = AssetBuilder.Instance.CustomSampleIcons[0];
        so.SampleData = _templateSampleObjects.Collection[0].SampleData;

        return item;
    }
}
