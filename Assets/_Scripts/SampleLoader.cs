using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SampleLoader : MonoBehaviour
{
    [SerializeField] private string BaseSampleDirectory;
    public List<AudioClip> BaseSamples = new List<AudioClip>();
    private void Start()
    {
        BaseSampleDirectory = $"{Application.streamingAssetsPath}{Path.DirectorySeparatorChar}Samples";
        UpdatePaths();
    }

    void UpdatePaths()
    {
        if (Directory.Exists(BaseSampleDirectory))
        {
            DirectoryInfo info = new DirectoryInfo(BaseSampleDirectory);

            FileInfo[] files = info.GetFiles().OrderBy(p => p.Name).ToArray();
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".wav"))
                {
                    StartCoroutine(LoadAudio(file.FullName));
                }
            }
        }
    }

    private IEnumerator LoadAudio(string path)
    {
        Debug.Log("RELOADING AUDIO");
        Debug.Log(path);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                BaseSamples.Add(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }
}
