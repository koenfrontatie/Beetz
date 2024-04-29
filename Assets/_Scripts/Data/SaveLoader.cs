using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;
    
    private string _projectPath;

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
        _projectPath = Path.Combine(Utils.ProjectSavepath, DataStorage.Instance.ProjectData.ID);
        
        var _filePath = Path.Combine(_projectPath, "ProjectData.json");
        
        Utils.CheckForCreateDirectory(_projectPath);

        SaveData(_filePath, DataStorage.Instance.ProjectData);
    }

    private bool SaveData<T>(string path, T data)
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
    /// <summary>
    /// This loads a json from a path and invokes Events.ProjectDataLoaded when ProjectData is deserialized.
    /// </summary>
    /// <param name="url">full path to json file</param>
    /// 
    public async void DeserializeProjectData(string url)
    {
        //_projectPath = Path.Combine(Utils.ProjectSavepath, DataStorage.Instance.ProjectData.ID);

        //var _filePath = Path.Combine(_projectPath, "ProjectData.json");

        //Utils.CheckForCreateDirectory(_projectPath);

        //SaveData(_filePath, DataStorage.Instance.ProjectData);

        string jsonData = await FetchJsonDataAsync(url);

        ProjectData myObject = await DeserializeJsonAsync(jsonData);

        Events.ProjectDataLoaded?.Invoke(myObject);

        Debug.Log($"Deserialized object: {myObject}");
    }

    private async Task<string> FetchJsonDataAsync(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                return null;
            }

            return www.downloadHandler.text;
        }
    }

    private async Task<ProjectData> DeserializeJsonAsync(string json)
    {
        return await Task.Run(() => JsonConvert.DeserializeObject<ProjectData>(json));
    }
    #endregion
    
    public string NewGuid()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<IDCollection> GetCustomSampleCollection()
    {
        var strings = new List<string>();

        var samplesPath = Path.Combine(Utils.ProjectSavepath, DataStorage.Instance.ProjectData.ID, "Samples");

        await Task.Run(() =>
        {
            var sortedFiles = new DirectoryInfo(samplesPath).GetDirectories().OrderBy(f => f.LastWriteTime).ToList();

            for(int i = 0; i < sortedFiles.Count; i++)
            {

                //if (sortedFiles[i].Name == "BaseSamples") continue;

                strings.Add(sortedFiles[i].Name);
            }



        });

        return new IDCollection(strings);
    }

    public async Task<Texture2D> GetIconFromGuid(string guid)
    {
        var samplesPath = Path.Combine(Utils.ProjectSavepath, DataStorage.Instance.ProjectData.ID, "Samples");

        var iconPath = Path.Combine(samplesPath, guid, "ico.png");

        //Texture2D iconTexture = new Texture2D(150, 150, TextureFormat.ARGB32, false);
        Debug.Log(iconPath);
        
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
