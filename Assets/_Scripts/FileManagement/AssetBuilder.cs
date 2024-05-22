
using FileManagement;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Un4seen.Bass.Misc.BaseEncoder;


public class AssetBuilder : MonoBehaviour
{
    public static AssetBuilder Instance;

    //[Header("Loaded Custom Sample GUID's")]
    //public IDCollection CustomSamples;

    [Header("Loaded Custom Sample Icons")]
    public List<Texture2D> CustomSampleIcons;

    //public SampleObject SelectedSampleObject;

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

    private Dictionary<string, Vector3[]> _meshDictionary = new Dictionary<string, Vector3[]>();


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

        FileManager.SampleDeleted += OnDeleteSample;

    }

    void OnDisable()
    {

        FileManager.SampleDeleted -= OnDeleteSample;

    }

    public void OnDeleteSample(string id)
    {
        if(_meshDictionary.ContainsKey(id))
        {
            _meshDictionary.Remove(id);
        }
    }

    public async void SearchForCustomSamples()
    {
        //CustomSamples.IDC.Clear();
        //CustomSamples.IDC.Count = 0;

        //CustomSamples = await SaveLoader.Instance.GetCustomSampleCollection();
        FindCustomTextures();
    }

    public async void FindCustomTextures()
    {
        CustomSampleIcons.Clear();

        for (int i = 0; i < FileManager.Instance.UniqueSamplePathCollection.Count; i++)
        {
            var pathToCheck = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);

            var icon = await SaveLoader.Instance.GetIconFromGuid(FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]));

            if (icon == null)
            {
                Debug.LogError("Icon is null");
                //CustomSampleIcons.Insert(i, await SaveLoader.Instance.GetIconFromGuid("2"));
                continue;
            }
            else
            {
                CustomSampleIcons.Insert(i, icon);
            }

        }

        Events.CustomDataLoaded?.Invoke();
    }

    public async Task<SampleObject> GetSampleObject(string guid)
    {
        if (guid.Length < 3) // if template
        {
            var copy = Instantiate(_templateSampleObjects.Collection[int.Parse(guid)]);
            //Debug.Log("less than 3");
            return copy;
        }
        else
        {
            SampleData sampleData = await GetSampleData(guid);
            Vector3[] vertices = await GetMeshData(guid);
            



            //set unique mesh
            SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);
            
            if(vertices != null)
            {
                copy.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh.SetVertices(vertices);
            }
            // set unique data
            copy.SampleData = sampleData;
            return copy;
            //throw new NotImplementedException();
        }
    }

    public async Task<SampleData> GetSampleData(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;
        
        if (guid.Length < 3) // if template
        {
            return _templateSampleObjects.Collection[int.Parse(guid)].SampleData;
        }
        else
        {
            var sampleData = await SaveLoader.Instance.DeserializeSampleData(Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json"));
            return sampleData;
            //return await SaveLoader.Instance.DeserializeSampleData(Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json"));
            //throw new NotImplementedException();
        }
    }

    public async Task<Vector3[]> GetMeshData(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;

        if (guid.Length < 3) // if template
        {
            return null;
        }
        else
        {
            var path = Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "verts.json");

            if (!File.Exists(path)) return null;
            
            if(_meshDictionary.TryGetValue(guid, out var verts))
            {
                return verts;
            }
            else
            {
                
                _meshDictionary.Add(guid, await SaveLoader.Instance.DeserializeMeshData(path));
                
                return _meshDictionary.GetValueOrDefault(guid, null);
            }  
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
