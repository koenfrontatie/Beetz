
using FileManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


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

    private bool _loadingSampleObject;

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
        FileManager.NewSampleSelected += OnNewSampleSelected;
    }

    private void OnNewSampleSelected(string guid)
    {
        // cancel getsampleobject and reinvoke with new guid
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

    public void SearchForCustomSamples()
    {
        //CustomSamples.IDC.Clear();
        //CustomSamples.IDC.Count = 0;

        //CustomSamples = await SaveLoader.Instance.GetCustomSampleCollection();
        FindCustomTextures();
    }

    //public async void FindCustomTextures()
    //{
    //    CustomSampleIcons.Clear();

    //    for (int i = 0; i < FileManager.Instance.UniqueSamplePathCollection.Count; i++)
    //    {
    //        var pathToCheck = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);

    //        Texture2D icon = await SaveLoader.Instance.GetIconFromGuid(FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]));

    //        if (icon == null)
    //        {
    //            Debug.LogError("Icon is null");
    //            //CustomSampleIcons.Insert(i, await SaveLoader.Instance.GetIconFromGuid("2"));
    //            continue;
    //        }
    //        else
    //        {
    //            CustomSampleIcons.Insert(i, icon);
    //        }

    //    }

    //    Events.CustomDataLoaded?.Invoke();
    //}

    public async void FindCustomTextures()
    {
        CustomSampleIcons.Clear();

        List<Texture2D> iconsToInsert = new List<Texture2D>();

        for (int i = 0; i < FileManager.Instance.UniqueSamplePathCollection.Count; i++)
        {
            var pathToCheck = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);

            Texture2D icon = await SaveLoader.Instance.GetIconFromGuid(pathToCheck);

            if (icon == null)
            {
                Debug.LogError("Icon is null");
                // Consider handling the null icon case differently, perhaps by logging or skipping
                continue;
            }
            else
            {
                iconsToInsert.Add(icon);
            }
        }

        // Merge the collected icons with CustomSampleIcons
        CustomSampleIcons.AddRange(iconsToInsert);

        Events.CustomDataLoaded?.Invoke();
    }


    public async Task<SampleObject> GetSampleObject(string guid)
    {
        if (_loadingSampleObject)
        {
            return null;
        }
        else
        {
            _loadingSampleObject = true;

            if (guid.Length < 3) // if template
            {
                SampleObject copy = Instantiate(_templateSampleObjects.Collection[int.Parse(guid)]);
                _loadingSampleObject = false;
                return copy;
            }
            else
            {
                SampleData sampleData = await GetSampleData(guid);
                Vector3[] vertices = await GetMeshData(guid);

                SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);

                //set unique mesh
                if (vertices != null)
                {
                    copy.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh.SetVertices(vertices);
                }

                // set unique data
                copy.SampleData = sampleData;
                
                _loadingSampleObject = false;
                return copy;
            }

        }
    }

    public async Task<List<GameObject>> GetSampleObjectCollection(string[] idCollection)
    {
        var sampleObjects = new List<GameObject>();

        var loadedVerts = new List<Vector3[]>();
        var loadedSampleData = new List<SampleData>();
        var loadedUniqueIds = new List<string>();

        for (int i = 0; i < idCollection.Length; i++)
        {
            if (idCollection[i].Length < 3) // if template
            {
                //Debug.Log($"Fetching template SampleObject for guid: {idCollection[i]}");
                SampleObject copy = Instantiate(_templateSampleObjects.Collection[int.Parse(idCollection[i])]);

                SampleData sampleData = _templateSampleData.Collection[int.Parse(idCollection[i])];

                sampleObjects.Add(copy.gameObject);
                copy.SampleData = sampleData;
            }
            else
            {
                if (!loadedUniqueIds.Contains(idCollection[i]))
                {

                    SampleData sampleData = await GetSampleData(idCollection[i]);
                    Vector3[] vertices = await GetMeshData(idCollection[i]);

                    SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);

                    //set unique mesh
                    if (vertices != null)
                    {
                        copy.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh.SetVertices(vertices);
                    }

                    // set unique data
                    copy.SampleData = sampleData;

                    sampleObjects.Add(copy.gameObject);

                    loadedSampleData.Add(sampleData);
                    loadedVerts.Add(vertices);
                    loadedUniqueIds.Add(idCollection[i]);
                }
                else
                {
                    var index = loadedUniqueIds.IndexOf(idCollection[i]);

                    SampleData sampleData = loadedSampleData[index];
                    Vector3[] vertices = loadedVerts[index];

                    SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);
                    sampleObjects.Add(copy.gameObject);
                }
            }

        }
        //Debug.Log($"idCollection[{i}] = {idCollection[i]}");

        for (int i = 0; i < loadedUniqueIds.Count; i++)
        {
            AddToDictionary(loadedUniqueIds[i], loadedVerts[i]);
        }

        return sampleObjects;
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
            SampleData sampleData = await SaveLoader.Instance.DeserializeSampleData(Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json"));
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
                
                AddToDictionary(guid, await SaveLoader.Instance.DeserializeMeshData(path));
                
                return _meshDictionary.GetValueOrDefault(guid, null);
            }  
        }
    }

    public void AddToDictionary(string guid, Vector3[] verts)
    {
        if (_meshDictionary.ContainsKey(guid))
        {
            _meshDictionary[guid] = verts;
        }
        else
        {
            _meshDictionary.Add(guid, verts);
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





//using FileManagement;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices.WindowsRuntime;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;
//using static Un4seen.Bass.Misc.BaseEncoder;


//public class AssetBuilder : MonoBehaviour
//{
//    public static AssetBuilder Instance;

//    //[Header("Loaded Custom Sample GUID's")]
//    //public IDCollection CustomSamples;

//    [Header("Loaded Custom Sample Icons")]
//    public List<Texture2D> CustomSampleIcons;

//    //public SampleObject SelectedSampleObject;

//    [Space(10)]
//    [Header("Template Prefabs")]
//    [SerializeField]
//    private SampleObjectCollection _templateSampleObjects;
//    [SerializeField]
//    private SampleDataCollection _templateSampleData;
//    [SerializeField]
//    private TextureCollection _templateIcons, _customIcons;

//    [SerializeField]
//    private GameObject _toolbarItemTemplate;

//    private Dictionary<string, Vector3[]> _meshDictionary = new Dictionary<string, Vector3[]>();

//    private bool _loadingSampleObject;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(this);
//        }
//        else
//        {
//            Instance = this;
//        }
//    }

//    private void OnEnable()
//    {

//        FileManager.SampleDeleted += OnDeleteSample;
//        FileManager.NewSampleSelected += OnNewSampleSelected;
//    }

//    private void OnNewSampleSelected(string guid)
//    {
//        // cancel getsampleobject and reinvoke with new guid
//    }

//    void OnDisable()
//    {

//        FileManager.SampleDeleted -= OnDeleteSample;

//    }

//    public void OnDeleteSample(string id)
//    {
//        if (_meshDictionary.ContainsKey(id))
//        {
//            _meshDictionary.Remove(id);
//        }
//    }

//    public void SearchForCustomSamples()
//    {
//        //CustomSamples.IDC.Clear();
//        //CustomSamples.IDC.Count = 0;

//        //CustomSamples = await SaveLoader.Instance.GetCustomSampleCollection();
//        FindCustomTextures();
//    }

    //public async void FindCustomTextures()
    //{
    //    CustomSampleIcons.Clear();

    //    for (int i = 0; i < FileManager.Instance.UniqueSamplePathCollection.Count; i++)
    //    {
    //        var pathToCheck = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);

    //        Texture2D icon = await SaveLoader.Instance.GetIconFromGuid(FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]));

    //        if (icon == null)
    //        {
    //            Debug.LogError("Icon is null");
    //            //CustomSampleIcons.Insert(i, await SaveLoader.Instance.GetIconFromGuid("2"));
    //            continue;
    //        }
    //        else
    //        {
    //            CustomSampleIcons.Insert(i, icon);
    //        }

    //    }

    //    Events.CustomDataLoaded?.Invoke();
    //}





//    public async void FindCustomTextures()
//    {
//        CustomSampleIcons.Clear();

//        List<Texture2D> iconsToInsert = new List<Texture2D>();

//        for (int i = 0; i < FileManager.Instance.UniqueSamplePathCollection.Count; i++)
//        {
//            var pathToCheck = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);

//            Texture2D icon = await SaveLoader.Instance.GetIconFromGuid(pathToCheck);

//            if (icon == null)
//            {
//                Debug.LogError("Icon is null");
//                // Consider handling the null icon case differently, perhaps by logging or skipping
//                continue;
//            }
//            else
//            {
//                iconsToInsert.Add(icon);
//            }
//        }

//        // Merge the collected icons with CustomSampleIcons
//        CustomSampleIcons.AddRange(iconsToInsert);

//        Events.CustomDataLoaded?.Invoke();
//    }

//    public async Task<List<GameObject>> GetSampleObjectCollection(string[] idCollection)
//    {
//        var sampleObjects = new List<GameObject>();

//        var loadedVerts = new List<Vector3[]>();
//        var loadedSampleData = new List<SampleData>();
//        var loadedUniqueIds = new List<string>();

//        for (int i = 0; i < idCollection.Length; i++)
//        {
//            if (idCollection[i].Length < 3) // if template
//            {
//                //Debug.Log($"Fetching template SampleObject for guid: {idCollection[i]}");
//                SampleObject copy = Instantiate(_templateSampleObjects.Collection[int.Parse(idCollection[i])]);

//                SampleData sampleData = _templateSampleData.Collection[int.Parse(idCollection[i])];

//                sampleObjects.Add(copy.gameObject);
//                copy.SampleData = sampleData;
//            }
//            else
//            {
//                if (!loadedUniqueIds.Contains(idCollection[i]))
//                {

//                    SampleData sampleData = await GetSampleData(idCollection[i]);
//                    Vector3[] vertices = await GetMeshData(idCollection[i]);

//                    SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);

//                    //set unique mesh
//                    if (vertices != null)
//                    {
//                        copy.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh.SetVertices(vertices);
//                    }

//                    // set unique data
//                    copy.SampleData = sampleData;

//                    sampleObjects.Add(copy.gameObject);

//                    loadedSampleData.Add(sampleData);
//                    loadedVerts.Add(vertices);
//                    loadedUniqueIds.Add(idCollection[i]);
//                }
//                else
//                {
//                    var index = loadedUniqueIds.IndexOf(idCollection[i]);

//                    SampleData sampleData = loadedSampleData[index];
//                    Vector3[] vertices = loadedVerts[index];

//                    SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);
//                    sampleObjects.Add(copy.gameObject);
//                }
//            }

//        }
//        //Debug.Log($"idCollection[{i}] = {idCollection[i]}");

//        for (int i = 0; i < loadedUniqueIds.Count; i++)
//        {
//            AddToDictionary(loadedUniqueIds[i], loadedVerts[i]);
//        }

//        return sampleObjects;
//    }

//    public async Task<SampleObject> GetSampleObject(string guid)
//    {
//        if (_loadingSampleObject)
//        {
//            return null;
//        }
//        else
//        {
//            _loadingSampleObject = true;

//            if (guid.Length < 3) // if template
//            {
//                SampleObject copy = Instantiate(_templateSampleObjects.Collection[int.Parse(guid)]);
//                _loadingSampleObject = false;
//                return copy;
//            }
//            else
//            {
//                SampleData sampleData = await GetSampleData(guid);
//                Vector3[] vertices = await GetMeshData(guid);

//                SampleObject copy = Instantiate(_templateSampleObjects.Collection[sampleData.Template]);

//                //set unique mesh
//                if (vertices != null)
//                {
//                    copy.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh.SetVertices(vertices);
//                }

//                // set unique data
//                copy.SampleData = sampleData;

//                _loadingSampleObject = false;
//                return copy;
//            }

//        }
//    }

//    public async Task<SampleData> GetSampleData(string guid)
//    {
//        if (string.IsNullOrEmpty(guid)) return null;

//        if (guid.Length < 3) // if template
//        {
//            return _templateSampleObjects.Collection[int.Parse(guid)].SampleData;
//        }
//        else
//        {
//            SampleData sampleData = await SaveLoader.Instance.DeserializeSampleData(Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json"));
//            return sampleData;
//            //return await SaveLoader.Instance.DeserializeSampleData(Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json"));
//            //throw new NotImplementedException();
//        }
//    }

//    public async Task<Vector3[]> GetMeshData(string guid)
//    {
//        if (string.IsNullOrEmpty(guid)) return null;

//        if (guid.Length < 3) // if template
//        {
//            return null;
//        }
//        else
//        {
//            var path = Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "verts.json");

//            if (!File.Exists(path)) return null;

//            if (_meshDictionary.TryGetValue(guid, out var verts))
//            {
//                return verts;
//            }
//            else
//            {

//                AddToDictionary(guid, await SaveLoader.Instance.DeserializeMeshData(path));

//                return _meshDictionary.GetValueOrDefault(guid, null);
//            }
//        }
//    }

//    public void AddToDictionary(string guid, Vector3[] verts)
//    {
//        if (_meshDictionary.ContainsKey(guid))
//        {
//            _meshDictionary[guid] = verts;
//        }
//        else
//        {
//            _meshDictionary.Add(guid, verts);
//        }
//    }

//    public async Task<GameObject> GetToolbarItem(string guid)
//    {
//        GameObject item = Instantiate(_toolbarItemTemplate);

//        var image = item.GetComponent<RawImage>();
//        var so = item.GetComponent<SampleObject>();

//        // if template

//        if (guid.Length < 3)
//        {
//            if (guid == "-1")
//            {
//                image.texture = null;
//                image.color = new Color(1, 1, 1, 0);
//                return item;
//            }

//            var template = int.Parse(guid);

//            image.texture = _templateIcons.Collection[template];

//            so.SampleData = _templateSampleData.Collection[template];

//            return item;
//        }
//        else
//        {

//            so.SampleData = await GetSampleData(guid);

//            image.texture = await SaveLoader.Instance.GetIconFromGuid(guid);

//            return item;
//        }
//    }
//}
