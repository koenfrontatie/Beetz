using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SampleDirectoryChecker : MonoBehaviour
{
    public List<string> SamplePaths = new List<string>();
    private int coroutineCounter, existingSamples;

    public int NumberOfBaseSamples;
    void Start()
    {
        // --------------------------------------------------- finds all basesamples in streamingassets
        var streamDir = new DirectoryInfo(Utils.StreamingBaseSamples);
        
        NumberOfBaseSamples = streamDir.GetFiles().Length;
        
        var targetDir = Utils.PersistentBaseSamples;
        
        Utils.CheckForCreateDirectory(targetDir);

        FileInfo[] files = streamDir.GetFiles();

        for (int i = 0; i < NumberOfBaseSamples; i++)
        {
            if (files[i].Name.EndsWith(".wav"))
            {
                // ---------------------------------------------- finds files to check in persistentdata
                StartCoroutine(CopyWavFile(files[i].FullName, files[i].Name ));
            }
        }
    }

    IEnumerator CopyWavFile(string path, string name)
    {
        string pathToCheck = Path.Combine(Utils.PersistentBaseSamples, name);
        SamplePaths.Add(pathToCheck);
        if (!File.Exists(pathToCheck))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(Path.Combine(path)))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(pathToCheck, www.downloadHandler.data);
                    Debug.Log("Copied " + name + " to persistentDataPath");
                }
                else
                {
                    Debug.LogError("Failed to copy " + name + ": " + www.error);
                }
            }
        }
        else
        {
            existingSamples++;
        }

        coroutineCounter--;
        if (coroutineCounter <= 0)
        {

            Events.OnBaseSamplesLoaded?.Invoke();
            if (existingSamples > 0) Debug.Log($"{existingSamples} already exist in persistent data.");
        }
    }


    //void Start()
    //{
    //    // --------------------------------------------------- finds all basesamples in streamingassets
    //    var dir = new DirectoryInfo(Utils.BaseSamplesPath);

    //    NumberOfBaseSamples = dir.GetFiles().Length;

    //    for (int i = 1; i <= NumberOfBaseSamples; i++)
    //    {
    //        var sampleDirectory = Path.Combine(Utils.BaseSamplesPath, i.ToString());
    //        var targetDirectory = Path.Combine(Utils.SampleSavepath, i.ToString());

    //        Debug.Log(targetDirectory);

    //        if (!Directory.Exists(targetDirectory))
    //        {
    //            Directory.CreateDirectory(targetDirectory);
    //        }

    //        DirectoryInfo info = new DirectoryInfo(sampleDirectory);
    //        FileInfo[] files = info.GetFiles();

    //        for (int j = 0; j < files.Length; j++)
    //        {
    //            if (files[j].Name.EndsWith(".wav"))
    //            {
    //                // ---------------------------------------------- finds substrings to check in persistentdata
    //                SampleSubStrings.Add(Path.Combine(info.Name, files[j].Name));
    //            }
    //        }
    //    }

    //    for (int i = 0; i < SampleSubStrings.Count; i++)
    //    {
    //        var pathToCheck = Path.Combine(Utils.SampleSavepath, SampleSubStrings[i]);
    //        StartCoroutine(CopyWavFile(SampleSubStrings[i]));
    //    }
    //}
}
