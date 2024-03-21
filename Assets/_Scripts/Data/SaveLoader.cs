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

        for(int i = 0; i < list.Count - 1; i++)
        {
            sb.Append($"{list[i]},");
        }

        var content = sb.ToString();

        if (!File.Exists(path)) File.Create(path).Close();

        StreamWriter sw = new StreamWriter(path);
        
        sw.Write(content);
        
        sw.Close();
    }

    public List<string> LoadProjectLibrary()
    {

        var content = File.ReadAllText($"{_projectPath}{Path.DirectorySeparatorChar}library.bfc");

        string[] entries = content.Split(",");

        List<string> loadedList = new List<string>();

        for(int i =0; i < entries.Length; i++)
        {
            loadedList.Add(entries[i]);
        }

        return loadedList;
    }
    
}
