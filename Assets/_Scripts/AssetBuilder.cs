
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

    public SampleObject SelectedSampleObject;

    [Space(10)]
    [Header("Template Prefabs")]
    [SerializeField]
    private SampleObjectCollection _templateSampleObjects;
    [SerializeField]
    private SampleDataCollection _templateSampleData;
    [SerializeField]
    private TextureCollection _templateIcons, _customIcons;

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
        //Events.SetSelectedSample += (so) =>
        //{
        //    SelectedGuid = so.SampleData.ID;
        //    SelectedSampleObject = so;
        //    Events.SampleSelected?.Invoke(SelectedSampleObject);
        //};

        Events.SetSelectedGuid += (guid) =>
        {
            SelectedGuid = guid;
        };

        Events.DeleteTile += OnDeleteTile;

    }

    void OnDisable()
    {
        //Events.SetSelectedSample += (so) =>
        //{
        //    SelectedGuid = so.SampleData.ID;
        //    SelectedSampleObject = so;
        //    Events.SampleSelected?.Invoke(SelectedSampleObject);
        //};

        Events.SetSelectedGuid -= (guid) =>
        {
            SelectedGuid = guid;
        };
        Events.DeleteTile -= OnDeleteTile;

    }

    void OnDeleteTile(string id)
    {
        for(int i = 0; i < CustomSamples.IDC.Count; i++)
        {
            if (CustomSamples.IDC[i] == id)
            {
                CustomSamples.IDC.RemoveAt(i);
                //CustomSampleIcons.RemoveAt(i);
                break;
            }
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S)) {
    //        SaveLoader.Instance.SaveData(Path.Combine(Utils.SampleSavepath, _templateSampleObjects.Collection[int.Parse(SelectedGuid)].SampleData.ID, ".json"), _templateSampleObjects.Collection[int.Parse(SelectedGuid)].SampleData);
    //    }
    //}

    public async void SearchForCustomSamples()
    {
        //CustomSamples.IDC.Clear();
        ////CustomSamples.IDC.Count = 0;

        //CustomSamples = await SaveLoader.Instance.GetCustomSampleCollection();
        //FindCustomTextures();
    }

    public async void FindCustomTextures()
    {
        //CustomSampleIcons.Clear();

        //for (int i = 0; i < CustomSamples.IDC.Count; i++)
        //{
        //    var icon = await SaveLoader.Instance.GetIconFromGuid(CustomSamples.IDC[i]);

        //    if (icon == null)
        //    {
        //        Debug.LogError("Icon is null");
        //        //CustomSampleIcons.Insert(i, await SaveLoader.Instance.GetIconFromGuid("2"));
        //        continue;
        //    } else
        //    {
        //        CustomSampleIcons.Insert(i, icon);
        //    }

        //}

        //Events.CustomDataLoaded?.Invoke();
    }

    public async Task<SampleObject> GetSampleObject(string guid)
    {
        if (guid.Length < 3) // if template
        {
            var copy = Instantiate(_templateSampleObjects.Collection[int.Parse(guid)]);
            return copy;
        }
        else
        {
            var sampleData = await GetSampleData(guid);
            
            var copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);

            copy.SampleData = sampleData;
            // set unique data
            return copy;
            //throw new NotImplementedException();
        }
    }

    public async void BioMakeUnique()
    {
        Events.BioMakeUnique?.Invoke(SelectedGuid);
    }

    public async Task<SampleData> GetSampleData(string guid)
    {
        if (guid.Length < 3) // if template
        {
            return _templateSampleObjects.Collection[int.Parse(guid)].SampleData;
        }
        else
        {
            var sampleData = await SaveLoader.Instance.DeserializeSampleData(Path.Combine(Utils.SampleSavepath, DataStorage.Instance.ProjectData.ID, guid, guid + ".json"));
            
            return sampleData;
            //throw new NotImplementedException();
        }
    }

    public async Task<GameObject> GetToolbarItem(string guid)
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

            so.SampleData = _templateSampleData.Collection[template];

            return item;
        }
        else
        {

            so.SampleData = await GetSampleData(guid);

            image.texture = await SaveLoader.Instance.GetIconFromGuid(guid);

            return item;
        }
    }
}
