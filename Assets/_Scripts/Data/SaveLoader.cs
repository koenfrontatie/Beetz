using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using UnityEngine.Networking;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;
    
    private string _projectPath;
    private IDataService _dataService = new JsonDataService();

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

    //public ProjectData LoadProjectData(string id)
    //{
    //    _relativeProjectPath = $"{Path.DirectorySeparatorChar}SaveFiles{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{id}";

    //    var path = $"{_relativeProjectPath}{Path.DirectorySeparatorChar}ProjectData.json";

    //    ProjectData data = _dataService.LoadData<ProjectData>(path);

    //    // if id  is null or smt create blank project?

    //    return data;
    //}

    //public ProjectData LoadProjectDataFullPath(string id)
    //{
    //    var path = id;

    //    ProjectData data = _dataService.LoadDataFullPath<ProjectData>(path);

    //    // if id  is null or smt create blank project?

    //    return data;
    //}

    //public void LoadProjectDataAsyncFullPath(string id, Action<ProjectData> callback)
    //{
    //    var path = id;

    //    _dataService.LoadDataAsyncFullPath<ProjectData>(path, callback);

    //    // if id  is null or smt create blank project?
    //}

    //public void LoadProjectInfoAsync(string id, Action<ProjectData> callback)
    //{
    //    _relativeProjectPath = $"{Path.DirectorySeparatorChar}SaveFiles{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{id}";
    //    var path = $"{_relativeProjectPath}{Path.DirectorySeparatorChar}ProjectData.json";

    //    try
    //    {
    //        _dataService.LoadDataAsync<ProjectData>(path, data =>
    //        {
    //            // Ensure callback is not null before invoking
    //            callback?.Invoke(data);
    //            Debug.Log("ProjectData loaded successfully.");
    //        });
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"Failed to load ProjectData: {e.Message}");
    //    }
    //}

    //public void SaveListToBfc(List<string> list)
    //{
    //    var path = $"{_projectPath}{Path.DirectorySeparatorChar}library.bfc";

    //    StringBuilder sb = new StringBuilder();

    //    for(int i = 0; i < list.Count; i++)
    //    {
    //        sb.Append($"{list[i]},");
    //    }

    //    if (sb.Length > 0)
    //    {
    //        sb.Remove(sb.Length - 1, 1);
    //    }

    //    var content = sb.ToString();

    //    File.WriteAllText(path, content);
    //}

    //public List<string> LoadProjectLibrary()
    //{
    //    var libraryPath = $"{_projectPath}{Path.DirectorySeparatorChar}library.bfc";

    //    if (!Utils.CheckForFile(libraryPath)) CreateLibrary();

    //    var content = File.ReadAllText(libraryPath);

    //    string[] entries = content.Split(",");

    //    List<string> loadedList = new List<string>();

    //    for(int i =0; i < entries.Length; i++)
    //    {
    //        loadedList.Add(entries[i]);
    //    }

    //    return loadedList;
    //}

    //void CreateLibrary()
    //{
    //    Debug.Log("Creating new library.");
    //    List<string> newLib = new List<string>();

    //    for (int i = 1; i <= 5; i++)
    //    {
    //        newLib.Add(i.ToString());
    //    }

    //    SaveListToBfc(newLib);
    //}

    //public async void InitializeProject()
    //{

    //}

    //public async Task LoadProjectInfo(string guid)
    //{
    //    var libraryPath = $"{_projectPath}{Path.DirectorySeparatorChar}ProjectInfo.json";
    //}


}
