using System.IO;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    public static SaveLoader Instance;

    public string ProjectId = "guid";
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

    private void Start()
    {
        SaveProjectConfiguration();
    }

    private void SaveProjectConfiguration()
    {
        var currentProjectPath = $"{Utils.ProjectSavepath}{Path.DirectorySeparatorChar}{ProjectId}";
        bool exists = System.IO.Directory.Exists(currentProjectPath);
        if(!exists)
        {
            System.IO.Directory.CreateDirectory($"{Utils.ProjectSavepath}{Path.DirectorySeparatorChar}{ProjectId}");
        }

    }

    void Update()
    {
        
    }
}
