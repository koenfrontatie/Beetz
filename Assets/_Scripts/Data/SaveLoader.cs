using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;
    public string ProjectId = "guid";
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

        _projectPath = $"{Utils.ProjectSavepath}{Path.DirectorySeparatorChar}{ProjectId}";
    }

    private void Start()
    {
        SaveProjectConfiguration();
    }
    private void SaveProjectConfiguration()
    {
        Utils.CheckForCreateDirectory(_projectPath);
    }

    public void SaveListToBfc(List<string> list)
    {
        var path = $"{_projectPath}{Path.DirectorySeparatorChar}library.bfc";

        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < list.Count; i++)
        {
            sb.Append($"{list[i]},");
        }

        if (sb.Length > 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }

        var content = sb.ToString();

        File.WriteAllText(path, content);
    }

    public List<string> LoadProjectLibrary()
    {
        var libraryPath = $"{_projectPath}{Path.DirectorySeparatorChar}library.bfc";

        if (!Utils.CheckForFile(libraryPath)) CreateLibrary();
        
        var content = File.ReadAllText(libraryPath);

        string[] entries = content.Split(",");

        List<string> loadedList = new List<string>();

        for(int i =0; i < entries.Length; i++)
        {
            loadedList.Add(entries[i]);
        }

        return loadedList;
    }

    void CreateLibrary()
    {
        Debug.Log("Creating new library.");
        List<string> newLib = new List<string>();

        for (int i = 1; i <= 5; i++)
        {
            newLib.Add(i.ToString());
        }

        SaveListToBfc(newLib);
    }
    
}
