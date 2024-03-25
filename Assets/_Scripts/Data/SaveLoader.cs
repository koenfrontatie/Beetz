using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;
    
    private string _relativeProjectPath;
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
    
    private void SaveProjectConfiguration()
    {
        Utils.CheckForCreateDirectory(Application.persistentDataPath + _relativeProjectPath);


        SerializeProjectInfo();
    }

    public void SerializeProjectInfo()
    {
        _relativeProjectPath = $"{Path.DirectorySeparatorChar}SaveFiles{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{DataLoader.Instance.ProjectData.ID}";

        var path = $"{_relativeProjectPath}{Path.DirectorySeparatorChar}ProjectInfo.json";
        
        _dataService.SaveData(path, DataLoader.Instance.ProjectData);
    }

    public ProjectData LoadProjectInfo(string id)
    {
        _relativeProjectPath = $"{Path.DirectorySeparatorChar}SaveFiles{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{id}";

        var path = $"{_relativeProjectPath}{Path.DirectorySeparatorChar}ProjectInfo.json";

        ProjectData info = _dataService.LoadData<ProjectData>(path);

        // if id  is null or smt create blank project?

        return info;
    }

    public void LoadProjectInfoAsync(string id, Action<ProjectData> callback)
    {
        _relativeProjectPath = $"{Path.DirectorySeparatorChar}SaveFiles{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{id}";
        var path = $"{_relativeProjectPath}{Path.DirectorySeparatorChar}ProjectData.json";

        try
        {
            _dataService.LoadDataAsync<ProjectData>(path, data =>
            {
                // Ensure callback is not null before invoking
                callback?.Invoke(data);
                Debug.Log("ProjectInfo loaded successfully.");
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load ProjectInfo: {e.Message}");
        }
    }


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
