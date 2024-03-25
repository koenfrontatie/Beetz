using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
public class JsonDataService : IDataService
{
    public T LoadData<T>(string relativePath)
    {
        string path = Application.persistentDataPath + relativePath;

        if(!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. Does not exist.");
            throw new FileNotFoundException();
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public void LoadDataAsync<T>(string relativePath, Action<T> callback)
    {
        string path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. Does not exist.");
            throw new FileNotFoundException();
        }

        try
        {
            // Read file asynchronously
            File.ReadAllTextAsync(path).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"Failed to load data due to: {task.Exception}");
                    throw new Exception("Failed to load data.");
                }

                // Deserialize data on the main thread
                T data = JsonConvert.DeserializeObject<T>(task.Result);

                // Invoke callback with loaded data
                Debug.Log($"Invoking {data} {callback}");
                callback?.Invoke(data);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public bool SaveData<T>(string relativePath, T data)
    {
        string path = Application.persistentDataPath + relativePath;

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
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Cannot save project data: {e.Message} {e.StackTrace}");
            return false;
        }
    }
}
