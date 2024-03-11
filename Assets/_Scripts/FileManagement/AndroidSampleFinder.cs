using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class AndroidSampleFinder : MonoBehaviour
{
    public List<string> BaseSampleNames = new List<string>();
    private int coroutineCounter, existingSamples;
    void Start()
    {
        coroutineCounter = BaseSampleNames.Count;
        existingSamples = 0;

        for(int i = 0; i < BaseSampleNames.Count; i++)
        {
            string fullName = BaseSampleNames[i] + ".wav";
            StartCoroutine(CopyWavFile(fullName));
        }
    }

    IEnumerator CopyWavFile(string fileName)
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string persistentDataPath = Application.persistentDataPath;
        string filePath = Path.Combine(streamingAssetsPath, fileName);
        string destinationPath = Path.Combine(persistentDataPath, fileName);
        

        if (!File.Exists(destinationPath))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllBytes(destinationPath, www.downloadHandler.data);
                    Debug.Log("Copied " + fileName + " to persistentDataPath");
                }
                else
                {
                    Debug.LogError("Failed to copy " + fileName + ": " + www.error);
                }
            }
        } else
        {
            existingSamples++;
        }

        coroutineCounter--;
        if (coroutineCounter <= 0) { 
        
            Events.OnBaseSamplesLoaded?.Invoke(); 
            if(existingSamples > 0) Debug.Log($"{existingSamples} already exist in persistent data.");
        }
    }
}
