using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using FileManagement;
using System.Collections.Generic;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;
    
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

    #region Serialization methods

    public void SerializeProjectData()
    {        
        SaveData(Path.Combine(FileManager.Instance.ProjectDirectory, "ProjectData.json"), DataStorage.Instance.ProjectData);
    }

    public bool SaveData<T>(string path, T data)
    {
        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data file exists. Deleting old file and writing new.");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Creating new data file");
            }

            using FileStream stream = File.Create(path);
            
            stream.Close();
            
            string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            
            File.WriteAllText(path, json);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Cannot save project data: {e.Message} {e.StackTrace}");
            
            return false;
        }
    }

    #endregion

    #region Deserialization methods

    public async Task<ProjectData> DeserializeProjectData(string jsonPath)
    {
#if !UNITY_EDITOR
        jsonPath = "File:///" + jsonPath;
#endif
        string jsonData = await FetchJsonDataAsync(jsonPath);

        ProjectData projectData = await Task.Run(() => JsonConvert.DeserializeObject<ProjectData>(jsonData));

        return projectData;
    }

    public async Task<ProjectData> DeserializeTemplateProjectData(string jsonPath)
    {
        string jsonData = await FetchJsonDataAsync(jsonPath);

        ProjectData projectData = await Task.Run(() => JsonConvert.DeserializeObject<ProjectData>(jsonData));

        return projectData;
    }

    public async Task<SampleData> DeserializeSampleData(string jsonPath)
    {
#if !UNITY_EDITOR
        jsonPath = "File:///" + jsonPath;
#endif
        string jsonData = await FetchJsonDataAsync(jsonPath);

        //SampleData sampleData = await Task.Run(() => JsonConvert.DeserializeObject<SampleData>(jsonData));

        return await Task.Run(() => JsonConvert.DeserializeObject<SampleData>(jsonData));
    }

    public async Task<Vector3[]> DeserializeMeshData(string jsonPath)
    {
#if !UNITY_EDITOR
        jsonPath = "File:///" + jsonPath;
#endif
        string jsonData = await FetchJsonDataAsync(jsonPath);

        //SampleData sampleData = await Task.Run(() => JsonConvert.DeserializeObject<SampleData>(jsonData));

        return await Task.Run(() => JsonConvert.DeserializeObject<Vector3[]>(jsonData));
    }

    private async Task<string> FetchJsonDataAsync(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("url is: " + url);
                Debug.LogError(www.error);
                return null;
            }

            return www.downloadHandler.text;
        }
    }

    #endregion

    public string NewGuid()
    {
        return Guid.NewGuid().ToString();
    }

    //public async Task<IDCollection> GetCustomSampleCollection()
    //{
    //    var strings = new List<string>();
    //    //Debug.Log($"samplespath= {samplesPath}");
    //    Utils.CheckForCreateDirectory(FileManager.Instance.UniqueSampleDirectory);

    //    await Task.Run(() =>
    //    {
    //        var sortedFiles = new DirectoryInfo(FileManager.Instance.UniqueSampleDirectory).GetDirectories().OrderBy(f => f.LastWriteTime).ToList();

    //        for(int i = 0; i < sortedFiles.Count; i++)
    //        {

    //            //if (sortedFiles[i].Name == "BaseSamples") continue;

    //            strings.Add(sortedFiles[i].Name);
    //        }



    //    });

    //    return new IDCollection(strings);
    //}

    public async Task<SampleData> GetCustomSampleData(string guid)
    {
        var samplePath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "SampleData.json");

        string jsonData = await FetchJsonDataAsync(samplePath);

        SampleData sampleData = await DeserializeSampleData(jsonData);

        return sampleData;
    }

    public async Task<Texture2D> GetIconFromGuid(string guid)
    {
        var iconPath = Path.Combine(FileManager.Instance.UniqueSampleDirectory, guid, "ico.png");
#if !UNITY_EDITOR
        iconPath = "File:///" + iconPath;
#endif
        //Texture2D iconTexture = new Texture2D(150, 150, TextureFormat.ARGB32, false);
        //Debug.Log("from save loader: " + iconPath);
        await Task.Yield();
        await Task.Delay(50);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(iconPath))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                return null;
            }
            Texture2D texture = DownloadHandlerTexture.GetContent(www);

            return texture;
        }
    }

    //-------------------------------- editor testing
    #region
    [SerializeField]
    private string _projectGuidToLoadFromEditor;

    public void LoadFromEditor()
    {
        var path = Path.Combine(Utils.ProjectSavepath, _projectGuidToLoadFromEditor, "ProjectData.json");
        DeserializeProjectData(path);
    }

    #endregion
}
